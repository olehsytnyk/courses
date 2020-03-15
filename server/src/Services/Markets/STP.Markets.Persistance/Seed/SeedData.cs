using Bogus;
using Microsoft.EntityFrameworkCore;
using STP.Markets.Domain;
using STP.Markets.Domain.Entities;
using STP.Markets.Persistance.Context;
using System;
using System.Threading.Tasks;

namespace STP.Markets.Persistance.Seed
{
    public static class SeedData
    {
        public static async Task EnsureSeedMarkets(MarketsDbContext content)
        {
            if (await content.Markets.AnyAsync())
                return;

            await content.AddRangeAsync(CreateMarkets());
            await content.SaveChangesAsync();
        }

        public static Market[] CreateMarkets()
        {
            var marketsFaker = new Faker<Market>().
                RuleFor(m => m.Id, f => 0).
                RuleFor(m => m.Name, f => f.Commerce.ProductName()).
                RuleFor(m => m.CompanyName, f => f.Company.CompanyName()).
                RuleFor(m => m.TickSize, f => Math.Round(f.Random.Double(), 5)).
                RuleFor(m => m.MinPrice, f => Math.Round(f.Random.Double() + f.Random.Number(100), 5)).
                RuleFor(m => m.MaxPrice, (f, m) => Math.Round(m.MinPrice + f.Random.Double() + f.Random.Number(10), 5)).
                RuleFor(m => m.Kind, f => f.Random.Enum<MarketKind>()).
                RuleFor(m => m.Currency, f => f.Random.Enum(Currency.Default));
            return marketsFaker.Generate(30).ToArray();
        }
    }
}
