using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using STP.Common.Exceptions;
using STP.Interfaces;
using STP.Interfaces.Enums;
using STP.Markets.Domain.Entities;
using STP.Markets.Infrastructure.Abstraction;
using Microsoft.Extensions.Logging;

namespace STP.Markets.MarketManagerService {
    public class MarketManager : IMarketManager {
        private readonly IMarketRepository _repository;
        private readonly IFileService _fileService;
        private readonly ILogger<MarketManager> _logger;

        public MarketManager(IMarketRepository repository, IFileService fileService, ILogger<MarketManager> logger) {
            _repository = repository;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<Market> GetByIdAsync(long id) {
            var item = await _repository.FindAsync(id);

            if (item == null)
                throw new NotFoundException(ErrorCode.MarketNotFount);

            return item;
        }

        public async Task<Market[]> GetMarketsAsync(Expression<Func<Market, bool>> expression, int skip, int take) {
            var items = await _repository.GetAsync(expression, skip, take);

            if (items.Length == 0)
                throw new NotFoundException(ErrorCode.MarketNotFount);

            return items;
        }

        public async Task<Market[]> GetMarketsAsync(Expression<Func<Market, bool>> expression) {
            var items = await _repository.GetAsync(expression);

            if (items.Length == 0)
                throw new NotFoundException(ErrorCode.MarketNotFount);

            return items;
        }

        public async Task<Stream> GetIconAsync(long id)
        {
            if (!await _repository.IsExistAsync(id))
                throw new NotFoundException(ErrorCode.MarketNotFount);

            var filePath = Path.Combine(AppContext.BaseDirectory, "Icons", Path.ChangeExtension(id.ToString(),"ico"));
            _logger.LogInformation($"Downloading market icon \nfrom path - {filePath}");
            return await _fileService.DownloadFileAsync(filePath);
        }
    }
}
