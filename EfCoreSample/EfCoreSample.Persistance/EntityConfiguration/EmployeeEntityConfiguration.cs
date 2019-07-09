using EfCoreSample.Doman;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreSample.Persistance.EntityConfiguration
{
    class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> employeeBuilder)
        {
            employeeBuilder.ToTable("employee", EfCoreSampleDbContext.SchemaName);

            employeeBuilder.HasKey(e => e.Id);

            employeeBuilder
                .HasMany(e => e.ReportsToEmployees)
                .WithOne(e => e.ReportsTo)
                .HasForeignKey(e => e.ReportsToId);

            employeeBuilder.Property(e => e.FirstName).HasMaxLength(128).IsRequired();
            employeeBuilder.Property(e => e.LastName).HasMaxLength(128);

            employeeBuilder.Property(t => t.LastModified)
                .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
                .ValueGeneratedOnAddOrUpdate();

            employeeBuilder.HasData(
                new Employee()
                {
                    Id = 1,
                    FirstName = "Petro",
                    LastName = "Petrenko"             
                },
                new Employee()
                {
                    Id = 2,
                    FirstName = "Olga",
                    LastName = "Petrenko"                    
                });
        }
    }
}
