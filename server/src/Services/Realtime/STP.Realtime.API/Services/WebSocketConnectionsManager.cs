using STP.Realtime.Abstraction;
using STP.Realtime.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using STP.Interfaces.Messages;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using Newtonsoft.Json;
using STP.Infrastructure;
using STP.Realtime.Common.Options;
using STP.Realtime.Common.WebSocketMessages;
using STP.Common.Models;
using FluentValidation;

namespace STP.Realtime.API.Services
{
    public struct UserAuthorizationInfo
    {
        public string UserId;
        public string SessionId;
        public UserAuthorizationInfo(string _userId, string _sessionId)
        {
            UserId = _userId;
            SessionId = _sessionId;
        }
    }
    internal class WebSocketConnectionsManager : IWebSocketConnectionsManager
    {
        private readonly ILogger<WebSocketConnectionsManager> _logger;
        private readonly ILogger<Room> _roomLogger;
        private readonly IMessageBus _messageBus;
        private readonly IValidator<SocketMessage> _socketMessageValidator;
        private readonly DatafeedHttpService _datafeedHttpService;

        private readonly ConcurrentDictionary<Guid, UserAuthorizationInfo> _connectionIdtoUserID = new ConcurrentDictionary<Guid, UserAuthorizationInfo>();
        private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _connections = new ConcurrentDictionary<Guid, IWebSocketConnection>();
        private readonly ConcurrentDictionary<string, Room> _rooms = new ConcurrentDictionary<string, Room>();

        public event EventHandler<IWebSocketConnection> Disconnect;
        public event EventHandler<IWebSocketConnection> Connect;

        public WebSocketConnectionsManager(IMessageBus messageBus,
                                           ILogger<WebSocketConnectionsManager> logger,
                                           ILogger<Room> roomLogger,
                                           DatafeedHttpService datafeedHttpService,
                                           IValidator<SocketMessage> socketMessageValidator)
        {
            _socketMessageValidator = socketMessageValidator;
            _messageBus = messageBus;
            _logger = logger;
            _roomLogger = roomLogger;
            _datafeedHttpService = datafeedHttpService;
            Disconnect += OnDisconnect;
        }
        void OnRoomEmpty(object sender, object args)
        {
            if (_rooms.TryRemove((sender as Room).RoomKey, out Room room))
            {
                room.RoomEmpty -= OnRoomEmpty;
                room.SubscriberRemoved -= OnRemoveSubscriber;
                Disconnect -= room.OnDisconnect;
                lock (room)
                {
                    _logger.LogInformation("Room {0} {1}:{2} removed(empty)", room.RoomName, room.SubjectAction, room.SubjectId);
                }
            }
        }
        void OnRemoveSubscriber(object obj, IWebSocketConnection webSocketConnection)
        {
            if ((obj as Room).RoomName == Exchange.Datafeed && (obj as Room).SubjectAction == SubjectAction.Update)
            {
                long marketId;
                try
                {
                    lock ((obj as Room).SubjectId)
                    {
                        marketId = long.Parse((obj as Room).SubjectId);
                    }
                    RemoveSubscriptionDatafeedAsync(marketId, webSocketConnection.SessionId);
                    _logger.LogInformation("OnRemoveSubscriber: Success.  client.Id={0} client.UserId={1} client.sessionId={2}",
                                                     webSocketConnection.Id, webSocketConnection.UserId, webSocketConnection.SessionId);
                }
                catch
                {
                    _logger.LogWarning("OnRemoveSubscriber: long.Parse fails client.Id={0} client.UserId={1} client.sessionId={2}",
                                                     webSocketConnection.Id, webSocketConnection.UserId, webSocketConnection.SessionId);
                }
            }
        }
        void OnDisconnect(object obj, IWebSocketConnection webSocketConnection)
        {
            _logger.LogInformation("WS {0} disconnected", webSocketConnection.Id);
            RemoveConnection(webSocketConnection.Id);
        }

        public async Task CreateWebSocketConnectionAsync(WebSocket webSocket,
                                              WebSocketConnectionsOptions options,
                                              Guid connectionId)
        {
            WebSocketConnection webSocketConnection = new WebSocketConnection(webSocket,
                 options.ReceivePayloadBufferSize, connectionId);
            webSocketConnection.ReceiveText += OnReceiveText;
            OnConnect(webSocketConnection);

            await webSocketConnection.ReceiveMessagesUntilCloseAsync();

            if (webSocketConnection.CloseStatus.HasValue)
            {
                await webSocket.CloseAsync(webSocketConnection.CloseStatus.Value, webSocketConnection.CloseStatusDescription, CancellationToken.None);
            }
            Disconnect?.Invoke(this, webSocketConnection);
            webSocketConnection.ReceiveText -= OnReceiveText;
        }
        public void OnConnect(IWebSocketConnection webSocketConnection)
        {
            AddConnection(webSocketConnection);
            Connect?.Invoke(this, webSocketConnection);
        }

        public async void OnReceiveText(object sender, string message)
        {
            _logger.LogInformation("WS {0} received message: {1}", (sender as WebSocketConnection).Id, message);
            SocketMessage socketMessage = null;
            SocketMessageResult socketMessageResult = new SocketMessageResult { Result = false };
            try
            {
                socketMessage = JsonConvert.DeserializeObject(message, typeof(SocketMessage)) as SocketMessage;
                if (!_socketMessageValidator.Validate(socketMessage).IsValid)
                    throw new Exception();
                socketMessageResult.RequestId = socketMessage.RequestId;
                socketMessageResult.Result = true;

                try
                {
                    socketMessageResult.Result = ProcessMessage(sender as IWebSocketConnection, socketMessage);
                }
                catch
                {
                    _logger.LogError("Internal error while processing Command procedure ");
                }
            }
            catch
            {
                socketMessageResult.RequestId = "Invalid Json Request";
            }
            _logger.LogInformation("WS {0} response.Result is {1}", (sender as WebSocketConnection).Id, socketMessageResult.Result);
            await (sender as WebSocketConnection).SendAsync(JsonConvert.SerializeObject(socketMessageResult), CancellationToken.None);
        }
        public bool IsValidConnectionId(Guid guid)
        {
            return _connectionIdtoUserID.ContainsKey(guid);
        }
        public Guid RegisterUserId(string userId, string sessionId)
        {
            Guid guid = Guid.NewGuid();
            _connectionIdtoUserID.TryAdd(guid, new UserAuthorizationInfo(userId, sessionId));
            return guid;
        }

        public void AddConnection(IWebSocketConnection connection)
        {
            _connectionIdtoUserID.TryRemove(connection.Id, out UserAuthorizationInfo userAuthorizationInfo);
            if (_connections.TryAdd(connection.Id, connection))
            {
                //if testUser => filling authorizationInfo
                if (userAuthorizationInfo.SessionId == null)
                {
                    userAuthorizationInfo.UserId = Guid.NewGuid().ToString();
                    userAuthorizationInfo.SessionId = Guid.NewGuid().ToString();
                }
                else
                    connection.UserId = userAuthorizationInfo.UserId;
                connection.SessionId = userAuthorizationInfo.SessionId;
                _logger.LogInformation("WS {0} is added, _connections.Count={1}", connection.Id, _connections.Count);
            }
            else
            {
                _logger.LogInformation("WS {0} NOT added, another copy of this connectionID detected", connection.Id);
            }
        }

        public void RemoveConnection(Guid connectionId)
        {
            IWebSocketConnection connection;
            _connections.TryRemove(connectionId, out connection);
            _logger.LogInformation("WS {0} removed, _connections.Count=" + connection.Id, _connections.Count);
        }

        public Task SendToAllAsync(string message, CancellationToken cancellationToken)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                connectionsTasks.Add(connection.SendAsync(message, cancellationToken));
            }
            return Task.WhenAll(connectionsTasks);
        }

        bool ProcessMessage(IWebSocketConnection webSocketConnection, SocketMessage socketMessage)
        {
            if (socketMessage.Action == SubsribeAction.Join) { return CommandJoin(webSocketConnection, socketMessage); }
            else
            if (socketMessage.Action == SubsribeAction.Leave) { return CommandLeave(webSocketConnection, socketMessage); }
            else
                return false;
        }
        bool CommandJoin(IWebSocketConnection webSocketConnection, SocketMessage socketMessage)
        {
            string debugMessage = string.Format("WS {0} initiated join to room. Exchange={1} routingKey={2}:{3}\n",
                                  webSocketConnection.Id, socketMessage.Subject, socketMessage.SubjectAction,
                                  socketMessage.SubjectId);
            //subscribe to identity server only with own user's ID
            if (socketMessage.Subject == Exchange.User && socketMessage.SubjectId != webSocketConnection.UserId)
            {
                debugMessage += string.Format("WS {0} join to room(exc.User) Fails, (User.ID!=message.SubjectId). Exchange={1} routingKey={2}:{3}\n", webSocketConnection.Id, socketMessage.Subject, socketMessage.SubjectAction, socketMessage.SubjectId);
                _logger.LogInformation(debugMessage);
                return false;
            }
            if (_rooms.TryGetValue(socketMessage.GetRoomKey(), out Room room))
            {
                debugMessage += "Room found!\n";
                if (room.IsPresent(webSocketConnection.Id))
                {
                    debugMessage += string.Format("WS {0} sending result True,(already in room). Exchange={1} routingKey={2}:{3}\n", webSocketConnection.Id, socketMessage.Subject, socketMessage.SubjectAction, socketMessage.SubjectId);
                    _logger.LogInformation(debugMessage);
                    return true;
                }
                else
                if (socketMessage.Subject == Exchange.Datafeed && socketMessage.SubjectAction == SubjectAction.Update &&
                     !TrySubscribeDatafeedAsync(webSocketConnection, socketMessage).GetAwaiter().GetResult())
                {
                    _logger.LogInformation(debugMessage);
                    return false;
                }
                room.AddSubscriber(webSocketConnection);
                debugMessage += string.Format("WS {0} initiated join to room. Exchange={1} routingKey={2}:{3}\n", webSocketConnection.Id, socketMessage.Subject, socketMessage.SubjectAction, socketMessage.SubjectId);
                _logger.LogInformation(debugMessage);
            }
            else
            {
                debugMessage += "Room not found! New room will created\n";
                if (socketMessage.Subject == Exchange.Datafeed && socketMessage.SubjectAction == SubjectAction.Update &&
                    !TrySubscribeDatafeedAsync(webSocketConnection, socketMessage).GetAwaiter().GetResult())
                {
                    _logger.LogInformation(debugMessage);
                    return false;
                }
                room = new Room(socketMessage.Subject, socketMessage.SubjectAction, socketMessage.SubjectId, socketMessage.GetRoomKey(),
                                        _messageBus, _roomLogger);
                if (_rooms.TryAdd(socketMessage.GetRoomKey(), room))
                {
                    Disconnect += room.OnDisconnect;
                    room.RoomEmpty += OnRoomEmpty;
                    room.SubscriberRemoved += OnRemoveSubscriber;
                    room.AddSubscriber(webSocketConnection);
                    debugMessage += string.Format(".Now we have {0} rooms\n", _rooms.Count);
                    _logger.LogInformation(debugMessage);
                }
                else
                {
                    //another thread already created room with this key. Need try all flow again
                    debugMessage += "another room instance was created with this room key, will try join to that room\n";
                    _logger.LogInformation(debugMessage);
                    return CommandJoin(webSocketConnection, socketMessage);
                }
            }
            return true;
        }
        bool CommandLeave(IWebSocketConnection webSocketConnection, SocketMessage socketMessage)
        {
            string routingKey = socketMessage.GetRoutingKey();
            string debugMessage = string.Format("WS {0} initiated leaving from room. Exchange={1} routingKey={2}\n", webSocketConnection.Id, socketMessage.Subject, routingKey);
            if (_rooms.TryGetValue(socketMessage.GetRoomKey(), out Room room))
            {
                debugMessage += "Room found\n";
                room.RemoveSubscriber(webSocketConnection);
            }
            else
            {
                debugMessage += "Room not found (user was not subsribed for that room?)\n";
            }
            _logger.LogInformation(debugMessage);
            return true;
        }
        private async Task<bool> TrySubscribeDatafeedAsync(IWebSocketConnection webSocketConnection, SocketMessage socketMessage)
        {
            long marketId;
            try
            {
                marketId = long.Parse(socketMessage.SubjectId);
            }
            catch
            {
                _logger.LogInformation("Fail convert to int, value=", socketMessage.SubjectId);
                return false;
            }
            bool result;
            try
            {
                result = await _datafeedHttpService.SubscribeMarketAsync(new DatafeedAction(marketId, webSocketConnection.SessionId));
            }
            catch
            {
                result = false;
                _logger.LogInformation("SubscribeMarketAsync fails(exception)");
            }
            _logger.LogInformation("SubscribeMarketAsync.result=", result);
            return result;
        }
        private async Task RemoveSubscriptionDatafeedAsync(long marketId, string sessionId)
        {
            try
            {
                await _datafeedHttpService.UnsubscribeMarketAsync(new DatafeedAction(marketId, sessionId));
            }
            catch
            {
                _logger.LogInformation("UnsubscribeMarketAsync fails(exception)");
            }
        }
    }
}
