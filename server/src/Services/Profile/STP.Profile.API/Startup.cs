using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STP.Infrastructure;
using STP.Infrastructure.Extensions;
using STP.Infrastructure.Setup;
using STP.Profile.Domain.DTO.Order;
using STP.Profile.Domain.DTO.Position;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Infrastructure;
using STP.Profile.Infrastructure.DataAccess;
using STP.Profile.Infrastructure.Managers;
using STP.Profile.Infrastructure.Validation;
using STP.Profile.Interfaces.DataAccess;
using STP.Profile.Interfaces.Managers;
using STP.Profile.Persistence.Context;
using System;
using STP.RabbitMq;
using STP.Profile.UpdateService.Abstract;
using STP.Profile.UpdateService;
using STP.Profile.Interfaces;

namespace STP.Profile.API
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(configuration, hostingEnvironment)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddAuthorization(opt => {
                opt.AddPolicy("internal", p => p.RequireClaim("client_id", "Inner"));
            });

            services.AddSingleton<IPositionCache, OpenPositionsCache>();
            services.AddSingleton<IUpdatesManager, UpdatesManager>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ITraderInfoRepository, TraderInfoRepository>();
            
            services.AddScoped<IOrderManager, OrderManager>();
            services.AddScoped<IPositionManager, PositionManager>();
            services.AddScoped<ITraderInfoManager, TraderInfoManager>();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ProfileDbContext>(options =>
                options.UseMySql(connectionString));

            services.AddTransient<IValidator<PositionFilterModel>, PositionFilterModelValidation>();
            services.AddTransient<IValidator<OrderFilterModel>, OrderFilterModelValidation>();
            services.AddTransient<IValidator<BaseOrderDTO>, BaseOrderDTOValidation>();
            services.AddTransient<IValidator<PostOrderDTO>, PostOrderDTOValidation>();
            services.AddTransient<IValidator<GetPositionDTO>, GetPositionDTOValidation>();
            services.AddHttpClient<IdentityHttpService>(client =>
            {
                client.BaseAddress = new Uri(_oauthOptions.AuthServer);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient<DatafeedHttpService>(client =>
            {
                client.BaseAddress = new Uri(_httpServiceOptions.ApiDatafeedUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            base.Configure(app, loggerFactory);

            app.EnsureContextMigrated<ProfileDbContext>();
        }
    }
}
