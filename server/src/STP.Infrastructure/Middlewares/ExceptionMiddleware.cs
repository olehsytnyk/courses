using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STP.Common.Exceptions;
using STP.Common.Models;
using STP.Infrastructure.ActionResults;
using STP.Common.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using STP.Interfaces.Enums;

namespace STP.Infrastructure.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IActionResultExecutor<ObjectResult> _actionResultExecutor;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IActionResultExecutor<ObjectResult> actionResultExecutor)
        {
            _logger = logger;
            _next = next;
            _actionResultExecutor = actionResultExecutor;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError($"Something went wrong: {ex} {ex.Error.Message} {ex.Error.StatusCode}");
                await SendResponseAsync(httpContext,new NotFoundObjectResult(ex.Error));
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError($"Something went wrong: {ex} {ex.Error.Message} {ex.Error.StatusCode}");
                await SendResponseAsync(httpContext, new BadRequestObjectResult(ex.Error));
            }
            catch (InvalidPermissionException ex)
            {
                _logger.LogError($"Something went wrong: {ex} {ex.Error.Message} {ex.Error.StatusCode}");
                await SendResponseAsync(httpContext, new ForbiddenObjectResult(ex.Error));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex} {ex.Message}");
                await SendResponseAsync(httpContext, new InternalServerErrorObjectResult(new ErrorDTO(ErrorCode.UnknownError, ex.Message)));
            }
        }

        /// <summary>
        /// Executes passed action result.
        /// </summary>
        /// <param name="context">HttpContext of current request.</param>
        /// <param name="objectResult">Instance of ObjectResult implementation, contains error data.</param>
        private Task SendResponseAsync(HttpContext context, ObjectResult objectResult) =>
                _actionResultExecutor.ExecuteAsync(new ActionContext() { HttpContext = context }, objectResult);

    }
}
