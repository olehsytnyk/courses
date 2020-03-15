using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STP.Datafeed.Application.Abstractions;
using STP.Datafeed.Infrastructure.Implements;
using STP.Infrastructure.Setup;
using STP.Infrastructure;
using STP.RabbitMq;

namespace STP.Datafeed.API
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
             : base(configuration, hostingEnvironment)
        {
        }

       
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            
            services.AddSingleton<IGeneratorService, GeneratorService>();
           
            

            services.AddHttpClient<MarketHttpService>(client =>
            {
                client.BaseAddress = new Uri(_httpServiceOptions.ApiMarketsUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddAuthorization(opt => {
                opt.AddPolicy("internal", p => p.RequireClaim("client_id", "Inner"));
            });
        }

        
        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            base.Configure(app, loggerFactory);

            app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()
                .ServiceProvider.GetRequiredService<IGeneratorService>().StartGeneratorAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}

