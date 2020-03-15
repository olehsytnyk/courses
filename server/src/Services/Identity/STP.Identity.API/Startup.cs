using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using STP.Common;
using STP.Common.Models;
using STP.Common.Options;
using STP.Identity.Domain.DTOs;
using STP.Identity.Domain.DTOs.User;
using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Abstraction;
using STP.Identity.Infrastructure.Mapping;
using STP.Identity.Infrastructure.Repository;
using STP.Identity.Infrastructure.Services;
using STP.Identity.Infrastructure.Validation;
using STP.Identity.Persistence.Context;
using STP.Identity.Persistence.Seed;
using STP.Infrastructure.FileService;
using STP.Infrastructure.Setup;
using STP.Infrastructure.Validation;
using STP.Interfaces;
using System;
using System.Data;

namespace STP.Identity.API
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            :base(configuration, hostingEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManagerService>()
                .AddDefaultTokenProviders();

            services.AddTransient<IValidator<CreateUserDto>, CreateUserValidator>();

            services.AddTransient<IValidator<UpdateUserDto>, UpdateUserValidatior>();

            services.AddTransient<IValidator<ChangePasswordDto>, ChangePasswordValidator>();

            services.AddTransient<IValidator<UploadFileDTO>, UploadFileValidator>();

            services.AddTransient<IDbConnection>((sp) => new MySqlConnection(connectionString));

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IUserAvatarRepository, UserAvatarRepository>();

            services.AddTransient<IUserAvatarManagerService, UserAvatarManagerService>();

            services.AddTransient<IFileService, FileService>();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddDeveloperSigningCredential()
            .AddAspNetIdentity<User>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseMySql(connectionString,
                        sql => sql.MigrationsAssembly("STP.Identity.Persistence"));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseMySql(connectionString,
                        sql => sql.MigrationsAssembly("STP.Identity.Persistence"));
                options.EnableTokenCleanup = true;
            });

            services.AddTransient<IProfileService, IdentityClaimsProfileService>();

            base.ConfigureServices(services);
            services.AddAuthorization(opt => {
                opt.AddPolicy("internal", p => p.RequireClaim("client_id", "Inner"));
            });
        }

        public override void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            base.Configure(app, loggerFactory);

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                SeedData.EnsureSeedData(scope.ServiceProvider).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            app.UseIdentityServer();
        }
    }
}
