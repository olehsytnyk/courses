using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using STP.Infrastructure.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using STP.Realtime.API.Middlewares;
using STP.Realtime.API.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Http;
using STP.Realtime.Common.Options;
using STP.Infrastructure;
using System;
using STP.Realtime.Common.Validation;
using STP.Realtime.Common.WebSocketMessages;
using FluentValidation;

namespace STP.Realtime.API
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) :
            base(configuration, hostingEnvironment)
        { }

        override public void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddWebSocketConnections();
            services.AddSingleton<IHostedService, KeepAliveService>();
            services.AddHttpClient<DatafeedHttpService>(client =>
            {
                client.BaseAddress = new Uri(_httpServiceOptions.ApiDatafeedUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient<ProfileHttpService>(client =>
            {
                client.BaseAddress = new Uri(_httpServiceOptions.ApiProfileUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddTransient<IValidator<SocketMessage>, SocketMessageValidator>();
        }

        override public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            base.Configure(app, loggerFactory);
            WebSocketConnectionsOptions webSocketConnectionsOptions = new WebSocketConnectionsOptions
            {
                SendSegmentSize = 4 * 1024
            };

            app
                .UseStaticFiles()
                .UseWebSockets()
                .MapWebSocketConnections("", webSocketConnectionsOptions)
                .Run(async (context) =>
                {
                    await context.Response.WriteAsync("test WebSocket");
                });

        }
    }
}
