using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Persistance.EntityConfiguration {
    public class WatchlistEntityConfiguration : IEntityTypeConfiguration<Watchlist> {
        public void Configure(EntityTypeBuilder<Watchlist> builder) {
            builder.ToTable("watchlists");

            builder.HasKey(i => i.Id);
        }
    }
}
