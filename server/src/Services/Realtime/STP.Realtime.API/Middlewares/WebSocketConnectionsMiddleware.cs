using Microsoft.AspNetCore.Http;
using STP.Interfaces.Enums;
using STP.Realtime.Abstraction;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using STP.Realtime.Common.Options;
using STP.Common.Exceptions;

namespace STP.Realtime.API.Middlewares
{
    internal class WebSocketConnectionsMiddleware
    {
        private WebSocketConnectionsOptions _options;
        private IWebSocketConnectionsManager _connectionsManager;
        private ILogger<WebSocketConnectionsMiddleware> _logger;
        public WebSocketConnectionsMiddleware(RequestDelegate next,
                                              WebSocketConnectionsOptions options,
                                              IWebSocketConnectionsManager connectionsService,
                                              ILogger<WebSocketConnectionsMiddleware> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connectionsManager = connectionsService ?? throw new ArgumentNullException(nameof(connectionsService));
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                Guid connectionId;
                try
                {   //for tests only
                    string s = context.Request.Path.Value.ToString();
                    s = s.ToLower();
                    if (s == "/test")
                    {
                        connectionId = Guid.NewGuid();
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await _connectionsManager.CreateWebSocketConnectionAsync(webSocket, _options, connectionId);
                        return;
                    }
                }
                catch { }
                if (Guid.TryParse(context.Request.Query["connectionId"], out connectionId))
                {
                    if (_connectionsManager.IsValidConnectionId(connectionId))
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await _connectionsManager.CreateWebSocketConnectionAsync(webSocket, _options, connectionId);
                        _logger.LogInformation("IP: {0} new websocket created, connectionId= {1}", context.Connection.RemoteIpAddress, connectionId);
                    }
                    else
                    {
                        _logger.LogInformation("IP: {0} has invalid requestId: {1}", context.Connection.RemoteIpAddress,
                            context.Request.Query["connectionId"]);
                        throw new InvalidPermissionException(ErrorCode.InvalidConnectionId);
                    }
                }
                else
                {
                    _logger.LogInformation("IP: {0} has no connectionId in request", context.Connection.RemoteIpAddress);
                    throw new InvalidDataException(ErrorCode.ConnectionIdNotFound);
                }
            }
        }
    }
}
