using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace STP.Infrastructure.DataAccess
{
    public class BaseDbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {
        public virtual T CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<T>();
            options.UseMySql(configuration.GetConnectionString("DefaulConnection"));
            return (T)Activator.CreateInstance(typeof(T), options.Options);
        }
    }
}
