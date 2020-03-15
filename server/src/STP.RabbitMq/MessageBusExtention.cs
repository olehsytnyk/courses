using Microsoft.Extensions.DependencyInjection;
using STP.Interfaces.Messages;

namespace STP.RabbitMq
{
    public static class MessageBusExtention
    {
        public static IServiceCollection AddRabbitMessageBus(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionService, ConnectionService>();
            services.AddSingleton<IMessageBus, MessageBus>();
            return services;
        }
    }
}
