using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STP.Common;
using STP.Common.Options;
using STP.RabbitMq;

namespace STP.Infrastructure.Extensions
{
    public static class OptionCollectionExtension
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<BaseOptions>(configuration);

            services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));

            services.Configure<OAuthOptions>(configuration.GetSection("OAuthOptions"));

            services.Configure<SwaggerOptions>(configuration.GetSection("SwaggerOptions"));

            services.Configure<TokenProviderOptions>(configuration.GetSection("TokenProviderOptions"));

            services.Configure<HttpServiceOptions>(configuration.GetSection("HttpServiceOptions"));

            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQOptions"));
        }
    }
}
