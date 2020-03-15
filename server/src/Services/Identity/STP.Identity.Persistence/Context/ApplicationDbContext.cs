using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using STP.Identity.Domain.Entities;
using STP.Identity.Persistence.EntityConfigurations;
using STP.Interfaces;

namespace STP.Identity.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>, IUnitOfWork
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAvatar> Avatars { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserEntityConfiguration());
            builder.ApplyConfiguration(new UserAvatarEntityConfiguration());
        }
    }
}
