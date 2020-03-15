using Microsoft.EntityFrameworkCore;
using STP.Interfaces;
using STP.Markets.Domain.Entities;
using STP.Markets.Persistance.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Markets.Persistance.Context {
    public class MarketsDbContext : DbContext, IUnitOfWork{
        public MarketsDbContext(DbContextOptions<MarketsDbContext> options) : base(options) { }

        public DbSet<Market> Markets { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<MarketWatchlist> MarketWatchlists { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new MarketEntityConfiguration());
            builder.ApplyConfiguration(new WatchlistEntityConfiguration());
            builder.ApplyConfiguration(new MarketWatchlistEntityConfiguration());
        }
    }
}
