using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EfCoreSample.Persistance
{
    class EfCoreDbContextFactory : IDesignTimeDbContextFactory<EfCoreSampleDbContext>
    {
        public EfCoreSampleDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EfCoreSampleDbContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("LocalConnection"));

            return new EfCoreSampleDbContext(optionsBuilder.Options);
        }
    }
}
