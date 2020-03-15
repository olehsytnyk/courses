using Microsoft.EntityFrameworkCore;
using STP.Interfaces;
using STP.Profile.Domain.Entities;
using STP.Profile.Persistence.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Profile.Persistence.Context
{
    public class ProfileDbContext : DbContext, IUnitOfWork
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options) { }

        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<PositionEntity> Positions { get; set; }
        public DbSet<TraderInfoEntity> TraderInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new OrderEntityConfiguration());
            builder.ApplyConfiguration(new PositionEntityConfiguration());
            builder.ApplyConfiguration(new TraderInfoEntityConfiguration());
        }
    }
}