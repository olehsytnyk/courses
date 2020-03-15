using Microsoft.Extensions.DependencyInjection;
using STP.Realtime.Abstraction;

namespace STP.Realtime.API.Services
{
    internal static class IServiceExtensions
    {
        public static IServiceCollection AddWebSocketConnections(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketConnectionsManager>();
            services.AddSingleton<IWebSocketConnectionsManager>(serviceProvider => serviceProvider.GetService<WebSocketConnectionsManager>());

            return services;
        }
    }
}

