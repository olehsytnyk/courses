using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Realtime.Abstraction;
using System.Security.Claims;
using STP.Common.Exceptions;
using STP.Interfaces.Enums;
using System;
using Microsoft.Extensions.Logging;

namespace STP.Realtime.API.Controllers
{
    [Authorize]
    [Route("api/realtime")]

    public class RealtimeController : Controller
    {
        private IWebSocketConnectionsManager _webSocketConnectionsManager;
        private ILogger<RealtimeController> _logger;
        public RealtimeController(IWebSocketConnectionsManager webSocketConnectionsManager,
                                  ILogger<RealtimeController> logger)
        {
            _webSocketConnectionsManager = webSocketConnectionsManager;
            _logger = logger;
        }

        /// <summary>
        /// Authorize User for receiving ConnectionID for WebSocketConnection
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/realtime/connect
        /// </remarks>
        /// <response code="200">If successful authorized. As result, we receiving connectionId </response>
        // <response code="403">If not authorized</response> 
        [HttpPost("connect")]
        public IActionResult GetConnectionId()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogDebug("IP: {0} userId is null", HttpContext.Connection.RemoteIpAddress);
                throw new InvalidPermissionException(ErrorCode.InvalidUserId);
            }
            string sessionId = User.FindFirst("session_id")?.Value;
            if (sessionId == null)
            {
                _logger.LogDebug("IP: {0} userId {1} session_id is null", HttpContext.Connection.RemoteIpAddress, userId);
                throw new InvalidPermissionException(ErrorCode.InvalidSessionId);
            }

            Guid guid = _webSocketConnectionsManager.RegisterUserId(userId, sessionId);
            _logger.LogDebug("IP: {0} connectionId: {1} assigned to user: {2} sessionId: {3}",
                             HttpContext.Connection.RemoteIpAddress, guid, userId, sessionId);
            return Json(guid);
        }

    }

}
