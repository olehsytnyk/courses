using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Profile.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Profile.Persistence.EntityConfiguration
{
    public class PositionEntityConfiguration : IEntityTypeConfiguration<PositionEntity>
    {
        public void Configure(EntityTypeBuilder<PositionEntity> builder)
        {
            builder.ToTable("position");

            builder.HasKey(p => p.Id);

            //builder.Property(p => p.Timestamp)
            //    .HasDefaultValueSql("current_timestamp(6) ON UPDATE current_timestamp(6)")
            //    .ValueGeneratedOnAddOrUpdate();
        }
    }
}
