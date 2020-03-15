using Microsoft.AspNetCore.Builder;
using STP.Infrastructure.Middlewares;

namespace STP.Infrastructure.Extensions
{
    static public class ExceptionMiddlewareExtension
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
