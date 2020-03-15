using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STP.Markets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Persistance.EntityConfiguration {
    class MarketWatchlistEntityConfiguration : IEntityTypeConfiguration<MarketWatchlist> {
        public void Configure(EntityTypeBuilder<MarketWatchlist> builder) {
            builder.HasKey(mw => new { mw.MarketId, mw.WatchlistId });

            builder.HasOne(mw => mw.Market)
                .WithMany(m => m.MarketWatchlists)
                .HasForeignKey(mw => mw.MarketId);

            builder.HasOne(mw => mw.Watchlist)
                .WithMany(w => w.MarketWatchlists)
                .HasForeignKey(mw => mw.WatchlistId);
        }
    }
}
