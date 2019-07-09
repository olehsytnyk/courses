using EfCoreSample.Doman;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreSample.Persistance.EntityConfiguration
{
    class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> addressBuilder)
        {
            addressBuilder.ToTable("address", EfCoreSampleDbContext.SchemaName);

            addressBuilder.HasKey(a => a.Id);

            addressBuilder
                .HasOne(a => a.Employee)
                .WithMany(e => e.Addresses)
                .HasForeignKey(a => a.EmployeeId);

            addressBuilder.HasIndex(a =>
            new
            {
                a.City,
                a.Country,
                a.Street
            }).ForMySqlIsFullText();

            addressBuilder.HasData(
                new Address()
                {
                    Id = 1,
                    Street = "Lvivs`ka",
                    City = "Ternopil",
                    Country = "Ukraine",
                    EmployeeId = 1
                },
                new Address()
                {
                    Id = 2,
                    Street = "Tarnavs`kogo",
                    City = "Ternopil",
                    Country = "Ukraine",
                    EmployeeId = 2
                });
        }
    }
}
