using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using STP.Infrastructure;
using STP.Infrastructure.FileService;
using STP.Infrastructure.Setup;
using STP.Interfaces;
using STP.Markets.API.Validators;
using STP.Markets.Application;
using STP.Markets.Infrastructure.Abstraction;
using STP.Markets.Infrastructure.Repository;
using STP.Markets.MarketManagerService;
using STP.Markets.Persistance.Context;
using STP.Markets.Persistance.Seed;
using STP.Markets.WatchlistManagerService;

namespace STP.Markets.API {
    public class Startup : BaseStartup {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) : base (configuration, hostingEnvironment) { }

        public override void ConfigureServices(IServiceCollection services) {
            base.ConfigureServices(services);

            services.AddDbContext<MarketsDbContext>(opt => opt.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IMarketRepository, MarketRepository>();
            services.AddScoped<IWatchlistRepository, WatchlistRepository>();

            services.AddScoped<IMarketManager, MarketManager>();
            services.AddScoped<IWatchlistManager, WatchlistManager>();
            services.AddScoped<IFileService, FileService>();

            services.AddTransient<IValidator<WatchlistPostDto>, WatchlistPostDtoValidator>();

            services.AddAuthorization(opt => {
                opt.AddPolicy("internal", p => p.RequireClaim("client_id", "Inner"));
            });

            services.AddHttpClient<IdentityHttpService>(client =>
            {
                client.BaseAddress = new Uri(_oauthOptions.AuthServer);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            base.Configure(app, loggerFactory);

            Migrate(app);
        }

        private void Migrate(IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                var marketContext = serviceScope.ServiceProvider.GetRequiredService<MarketsDbContext>();

                marketContext.Database.Migrate();
                SeedData.EnsureSeedMarkets(marketContext).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}