using System;
using STP.Realtime.Abstraction;
using System.Threading;
using System.Threading.Tasks;
using STP.Interfaces.Messages;
using Newtonsoft.Json;
using System.Text;
using STP.Realtime.Common.WebSocketMessages;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace STP.Realtime.Infrastructure
{
    public class Room : IMessageHandler
    {
        private readonly ILogger<Room> _logger;
        private readonly IMessageBus _messageBus;
        private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _subscribers = new ConcurrentDictionary<Guid, IWebSocketConnection>();

        public Exchange RoomName { get; }
        public SubjectAction SubjectAction { get; }
        public string SubjectId { get; }
        public string RoomKey { get; }

        public event EventHandler RoomEmpty;
        public event EventHandler<IWebSocketConnection> SubscriberRemoved;


        private string RoutingKey() { return SubjectAction.ToString() + ":" + SubjectId; }
        private string ExchangeName() { return "exc." + RoomName.ToString(); }
        private void _log(string logMessage)
        {
            _logger.LogInformation("RoomName: {0} Key: {1} -> {2}", RoomName, RoutingKey(), logMessage);
        }

        public Room(Exchange roomName, SubjectAction subjectAction, string subjectId, string roomKey, IMessageBus messageBus, ILogger<Room> logger)
        {
            RoomName = roomName;
            SubjectAction = subjectAction;
            SubjectId = subjectId;
            _messageBus = messageBus;
            _logger = logger;
            RoomKey = roomKey;
            AddSubscriptionToMessageBus();
        }
        public void AddSubscriber(IWebSocketConnection subscriber)
        {
            //checking for clones was in RoomManager.CommandJoin
            if (_subscribers.TryAdd(subscriber.Id, subscriber))
            {
                _log(string.Format("connection {0} added. Now {1} connections in the room",
                                   subscriber.Id,
                                   _subscribers.Count));
            }
            else
            {
                _log(string.Format("connection {0} NOT added. Already in the room", subscriber.Id));
            }
        }

        public void OnDisconnect(object obj, IWebSocketConnection webSocketConnection)
        {
            RemoveSubscriber(webSocketConnection);
        }

        public void _subscribersRemoved()
        {
            if (_subscribers.Count == 0)
                UnsubscribeFromMessageBus();
            RoomEmpty?.Invoke(this, null);
        }

        public void RemoveSubscriber(IWebSocketConnection webSocketConnection)
        {
            if (_subscribers.TryRemove(webSocketConnection.Id, out IWebSocketConnection removedWebSocketConnection))
            {

                SubscriberRemoved?.Invoke(this, removedWebSocketConnection);
                _log(string.Format("connection {0} removed. Now {1} connections in the room",
                                   removedWebSocketConnection.Id,
                                   _subscribers.Count));
                _subscribersRemoved();
            }
            else
            {
                _log(string.Format("connection not removed(not exists). Now {1} connections in the room",
                                   webSocketConnection.Id,
                                   _subscribers.Count));
            }
        }
        void AddSubscriptionToMessageBus()
        {
            _messageBus.Subscribe<IMessage, Room>(this, ExchangeName(), Interfaces.Enums.RabbitExchangeType.DirectExchange, RoutingKey());
        }

        void UnsubscribeFromMessageBus()
        {
            _messageBus.Unsubscribe<IMessage, Room>(ExchangeName(), RoutingKey());
        }
        public async Task HandleAsync(byte[] message)
        {
            SocketMessageTransfer messageToSend = new SocketMessageTransfer();
            messageToSend.Subject = RoomName;
            messageToSend.SubjectId = RoutingKey();
            messageToSend.Data = Encoding.UTF8.GetString(message);
            string convertedMessage = JsonConvert.SerializeObject(messageToSend);
            _log(string.Format("RabbitMQ message received"));

            ICollection<IWebSocketConnection> list = _subscribers.Values;
            if (list != null)
            {
                _logger.LogInformation("{0} sending message to {1} subscribers", RoomKey, _subscribers.Count);
                foreach (IWebSocketConnection webSocketConnection in list)
                {
                    if (webSocketConnection != null)
                    {
                        Task.Run(() => webSocketConnection.SendAsync(convertedMessage, CancellationToken.None));
                    }
                }
            }
            else
            {
                _logger.LogError("{0} _subscribers.Values=null!??", RoomKey);
            }
        }
        public async Task HandleAsync(IMessage message)
        {
            _logger.LogError("{0} Received Message via IMessage interface(WRONG!)", RoomKey);
        }
        public bool IsPresent(Guid guid)
        {
            return _subscribers.ContainsKey(guid);
        }
    }
}
