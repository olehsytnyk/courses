using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Markets.Domain;
using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Persistance.EntityConfiguration {
    class MarketEntityConfiguration : IEntityTypeConfiguration<Market> {
        public void Configure(EntityTypeBuilder<Market> builder) {
            builder.ToTable("markets");

            builder.HasKey(i => i.Id);
        }
    }
}
