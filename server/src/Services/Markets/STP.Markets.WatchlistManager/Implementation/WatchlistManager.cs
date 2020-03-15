using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using STP.Common.Exceptions;
using STP.Infrastructure;
using STP.Interfaces.Enums;
using STP.Markets.Domain.Entities;
using STP.Markets.Infrastructure.Abstraction;

namespace STP.Markets.WatchlistManagerService
{
    public class WatchlistManager : IWatchlistManager {
        private readonly IWatchlistRepository _repository;
        private readonly IMarketRepository _marketRepository;
        private readonly IdentityHttpService _identityHttpService;

        public WatchlistManager(IWatchlistRepository repository, IMarketRepository marketRepository, IdentityHttpService identityHttpService) {
            _repository = repository;
            _marketRepository = marketRepository;
            _identityHttpService = identityHttpService;
        }

        public async Task<Watchlist> CreateAsync(Watchlist item) {

            if (!await _identityHttpService.IsExistUserAsync(item.UserId))
                throw new InvalidDataException(ErrorCode.UserNotFound);

            foreach(var i in await _repository.GetAsync(w => w.UserId == item.UserId)) {
                if (i.Name == item.Name)
                    throw new InvalidDataException(ErrorCode.InvalidMarket, "Watchlist name must be unique");
            }

            foreach (var mw in item.MarketWatchlists)
                if (!await _marketRepository.IsExistAsync(mw.MarketId))
                    throw new NotFoundException(ErrorCode.MarketNotFount);

            var entity = await _repository.InsertAsync(item);
            await _repository.UnitOfWork.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Watchlist item) {
            bool result = _repository.Remove(item);
            if (result) {
                await _repository.UnitOfWork.SaveChangesAsync();
                return result;
            } else {
                throw new NotFoundException(ErrorCode.MarketNotFount, "Watchlist not found");
            }
        }

        public async Task<bool> DeleteAsync(long id) {
            bool result = _repository.Remove(id);
            if (result) {
                await _repository.UnitOfWork.SaveChangesAsync();
                return result;
            } else {
                throw new NotFoundException(ErrorCode.MarketNotFount, "Watchlist not found");
            }
        }

        public async Task<Watchlist> GetByIdAsync(long id) {
            var item = await _repository.FindAsync(id);

            if (item == null)
                throw new NotFoundException(ErrorCode.MarketNotFount, "Watchlist not found");

            return item;
        }

        public async Task<Watchlist[]> GetWatchlistsAsync(Expression<Func<Watchlist, bool>> expression, int skip, int take) {
            var items = await _repository.GetAsync(expression, skip, take);

            if (items.Length == 0)
                throw new NotFoundException(ErrorCode.MarketNotFount, "Watchlists not found");

            return items;
        }

        public async Task RemoveMarketsAsync(IEnumerable<MarketWatchlist> items) {
            await _repository.RemoveMarketsAsync(items);
            await _repository.UnitOfWork.SaveChangesAsync();
        }

        public async Task<Watchlist> UpdateAsync(Watchlist item) {
            if (item.MarketWatchlists.Count != item.MarketWatchlists.Distinct((i1, i2) => i1.MarketId == i2.MarketId).Count())
                throw new InvalidDataException(ErrorCode.InvalidMarket, "Markets must be unique");

            foreach (var watchlist in await _repository.GetAsync(w => w.Id != item.Id && w.UserId == item.UserId)) {
                if (watchlist.Name == item.Name)
                    throw new InvalidDataException(ErrorCode.InvalidMarket, "Watchlist name must be unique");
            }

            foreach (var mw in item.MarketWatchlists)
                if (!await _marketRepository.IsExistAsync(mw.MarketId))
                    throw new NotFoundException(ErrorCode.MarketNotFount);

            Watchlist result = _repository.Update(item);
            await _repository.UnitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
