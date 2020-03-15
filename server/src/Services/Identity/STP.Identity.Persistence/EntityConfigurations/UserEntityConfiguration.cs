using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Identity.Domain.Entities;

namespace STP.Identity.Persistence.EntityConfigurations
{
    class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> userBuilder)
        {
            userBuilder.HasOne(u => u.UserAvatar).WithOne(a => a.User);

            userBuilder
            .HasOne(s => s.UserAvatar)
            .WithOne(ad => ad.User)
            .HasForeignKey<UserAvatar>(ad => ad.UserId);

            userBuilder.Property(u => u.UserName).HasMaxLength(20).IsRequired();

            userBuilder.Property(u => u.Email).HasMaxLength(30);

            userBuilder.Property(u => u.FirstName).HasMaxLength(30);
            userBuilder.Property(u => u.LastName).HasMaxLength(30);

            userBuilder.HasIndex(a =>
            new
            {
                a.UserName,
                a.FirstName,
                a.LastName,
                a.Email
            }).ForMySqlIsFullText();
        }
    }
}
