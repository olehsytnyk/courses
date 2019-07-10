using EfCoreSample.Doman;
using EfCoreSample.Doman.Entities;
using EfCoreSample.Persistance.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EfCoreSample.Persistance
{
    public class EfCoreSampleDbContext : DbContext
    {
        public const string SchemaName = "efcoresample";

        public EfCoreSampleDbContext(DbContextOptions<EfCoreSampleDbContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new AddressEntityConfiguration());
            builder.ApplyConfiguration(new EmployeeDepartmentEntityConfiguration());
            builder.ApplyConfiguration(new EmployeeEntityConfiguration());
            builder.ApplyConfiguration(new ProjectEntityConfiguration());
        }
    }
}