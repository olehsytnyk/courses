using STP.Realtime.Abstraction;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Realtime.Infrastructure
{
    public class WebSocketConnection : IWebSocketConnection
    {
        private WebSocket _webSocket;
        private int _receivePayloadBufferSize;

        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public WebSocketCloseStatus? CloseStatus { get; set; } = null;
        public string CloseStatusDescription { get; private set; } = null;

        public event EventHandler<string> ReceiveText;
        public WebSocketConnection(WebSocket webSocket, int receivePayloadBufferSize, Guid ConnectionId)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            _receivePayloadBufferSize = receivePayloadBufferSize;
            Id = ConnectionId;
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            await _webSocket.SendAsync(
                Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, cancellationToken);
        }

        public async Task SendAsync(byte[] message, CancellationToken cancellationToken)
        {
             _webSocket.SendAsync(message, WebSocketMessageType.Text, true, cancellationToken);
        }

        public async Task ReceiveMessagesUntilCloseAsync()
        {
            try
            {
                byte[] receivePayloadBuffer = new byte[_receivePayloadBufferSize];
                WebSocketReceiveResult webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);
                while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                {
                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Binary)
                    {
                        throw (new Exception("WebSocket.MessageType.Binary is not allowed"));
                    }
                    else
                    {
                        string webSocketMessage = Encoding.UTF8.GetString(receivePayloadBuffer).TrimEnd((char)0);
                        Array.Clear(receivePayloadBuffer, 0, webSocketMessage.Length);
                        OnReceiveText(webSocketMessage);
                    }

                    webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);
                }

                CloseStatus = webSocketReceiveResult.CloseStatus.Value;
                CloseStatusDescription = webSocketReceiveResult.CloseStatusDescription;
            }
            catch (WebSocketException wsex) when (wsex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            { }
        }

        private void OnReceiveText(string webSocketMessage)
        {
            ReceiveText?.Invoke(this, webSocketMessage);
        }
    }
}
