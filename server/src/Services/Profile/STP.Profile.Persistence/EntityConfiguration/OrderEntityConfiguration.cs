using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Profile.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Profile.Persistence.EntityConfiguration
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(o => o.Id);

            //builder.Property(o => o.Timestamp)
            //    .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
            //    .ValueGeneratedOnAddOrUpdate();
        }
    }
}