using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Markets.Application;
using STP.Markets.Domain.Entities;
using STP.Markets.MarketManagerService;

namespace STP.Markets.API.Controllers {
    [Route("api/markets")]
    [ApiController]
    [Produces("application/json")]
    public class MarketsController : ControllerBase {
        private readonly IMapper _mapper;
        private readonly IMarketManager _manager;

        public MarketsController(IMapper mapper, IMarketManager manager) {
            _mapper = mapper;
            _manager = manager;
        }

        /// <summary>
        /// Get market by id.
        /// </summary>
        /// <returns>A market on success. 3404 status code if market with given id does not exist.</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MarketDto>> GetById(long id) {
            Market market = await _manager.GetByIdAsync(id);
            return _mapper.Map<MarketDto>(market);
        }

        /// <summary>
        /// Get markets that match filter. If filter's field is empty it will not be applied.
        /// </summary>
        /// <returns>A markets on success. 3404 status code if any markets do not match filter.</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarketDto>>> GetAsync([FromQuery] MarketFilterDto filter) {
            Expression<Func<Market, bool>> expression = m =>
                (string.IsNullOrEmpty(filter.Name) || m.Name.Contains(filter.Name)) &&
                (string.IsNullOrEmpty(filter.CompanyName) || m.CompanyName.Contains(filter.CompanyName)) &&
                (filter.Kind == 0 || m.Kind == filter.Kind) &&
                (filter.Currency == 0 || m.Currency == filter.Currency);

            var markets = await _manager.GetMarketsAsync(expression, filter.Skip, filter.Take);

            return _mapper.Map<IEnumerable<Market>, IEnumerable<MarketDto>>(markets).ToList();
        }

        /// <summary>
        /// Returns all markets
        /// </summary>
        /// <remarks>For internal use only</remarks>
        [Authorize(Policy = "internal")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<MarketInnerDto>>> GetAllAsync() {
            var markets = await _manager.GetMarketsAsync(i => true);
            return _mapper.Map<IEnumerable<Market>, IEnumerable<MarketInnerDto>>(markets).ToList();
        }

        /// <summary>
        /// Get market`s icon by id.
        /// </summary>
        /// <returns>A Stream on success. 3404 status code if market with given id does not exist.</returns>
        //[Authorize]
        [HttpGet("{id}/icon")]
        public async Task<Stream> GetIconAsync([FromRoute]long id)
        {
            return await _manager.GetIconAsync(id);
        }
    }
}
