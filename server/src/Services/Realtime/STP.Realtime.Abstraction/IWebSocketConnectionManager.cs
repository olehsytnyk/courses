using System;
using System.Threading;
using System.Threading.Tasks;
using STP.Interfaces.Messages;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using STP.Realtime.Common.Options;

namespace STP.Realtime.Abstraction
{
    public interface IWebSocketConnectionsManager
    {
        Task CreateWebSocketConnectionAsync(WebSocket webSocket,
                                            WebSocketConnectionsOptions options,
                                            Guid connectionId);
        void AddConnection(IWebSocketConnection connection);
        void RemoveConnection(Guid connectionId);
        Task SendToAllAsync(string message, CancellationToken cancellationToken);
        Guid RegisterUserId(string userId, string sessionId);
        bool IsValidConnectionId (Guid guid);
        void OnReceiveText(object sender, string message);
        event EventHandler<IWebSocketConnection> Disconnect;
        event EventHandler<IWebSocketConnection> Connect;
     //   void OnDisconnect(IWebSocketConnection webSocketConnection);
        void OnConnect(IWebSocketConnection webSocketConnection);
    }
}
