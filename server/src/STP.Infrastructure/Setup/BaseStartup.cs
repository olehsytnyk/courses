using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using STP.Common.Options;
using STP.Infrastructure.Extensions;
using STP.RabbitMq;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SwaggerOptions = STP.Common.Options.SwaggerOptions;

namespace STP.Infrastructure.Setup
{
    public abstract class BaseStartup
    {
        public BaseStartup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;

            _swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(_swaggerOptions);

            _oauthOptions = new OAuthOptions();
            Configuration.GetSection(nameof(OAuthOptions)).Bind(_oauthOptions);

            _httpServiceOptions = new HttpServiceOptions();
            Configuration.GetSection(nameof(HttpServiceOptions)).Bind(_httpServiceOptions);
        }

        public HttpServiceOptions _httpServiceOptions { get; set; }
        public SwaggerOptions _swaggerOptions { get; set; }
        public OAuthOptions _oauthOptions { get; set; }
        protected IConfiguration Configuration { get; }
        protected IHostingEnvironment HostingEnvironment { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddRabbitMessageBus();

            services.ConfigureOptions(Configuration);

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            services.AddMvc().AddFluentValidation().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
           
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // base-address of your identityserver
                options.Authority = _oauthOptions.AuthServer;

                // name of the API resource
                options.Audience = _oauthOptions.ApiName;

                options.RequireHttpsMetadata = false;
            });

            services.AddHttpClient<TokenProvider>(client => 
            {
                client.BaseAddress = new Uri(_oauthOptions.AuthServer);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            UseSerilog(services);
            ConfigureServicesSwagger(services);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public virtual void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCors("MyPolicy");
            app.UseCustomExceptionMiddleware();
            app.UseAuthentication();
            loggerFactory.AddSerilog();
            UseConfigureSwagger(app);
            app.UseMvc();
        }

        private void ConfigureServicesSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_swaggerOptions.Version, new Info { Title = _oauthOptions.ApiName, Version = _swaggerOptions.Version });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "oauth2", Enumerable.Empty<string>() }
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "password", // just get token via browser (suitable for swagger SPA)
                    TokenUrl = Path.Combine(_oauthOptions.AuthServer, "connect", "token"),
                    Scopes = _swaggerOptions.SwaggerScopes
                });

                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });
        }

        private void UseConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
           
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(_swaggerOptions.UiEndpoint, _swaggerOptions.Description);
                options.RoutePrefix = "swagger";
                options.OAuthClientSecret("secret");
                options.OAuthClientId("Swagger");
                options.OAuthAppName(_oauthOptions.ApiName); // presentation purposes only
            });
        }

        private void UseSerilog(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
            //.SetBasePath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config"))
            .AddJsonFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config", "serilogConfig.json"))
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog());
        }
    }
}