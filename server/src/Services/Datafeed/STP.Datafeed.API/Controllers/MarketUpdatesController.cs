using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Common.Models;
using STP.Datafeed.Application.Abstractions;
using STP.Datafeed.Application.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace STP.Datafeed.API.Controllers.V1
{
    [Authorize(Policy = "internal")]
    [Route("api/market-updates")]
    [ApiController]
    [Produces("application/json")]
    public class MarketUpdatesController : Controller
    {
        private IGeneratorService _generatorService;
        private IMapper _mapper;

        public MarketUpdatesController(IGeneratorService generatorService, IMapper mapper)
        {
            _generatorService = generatorService;
            _mapper = mapper;
        }

        /// <summary>
        /// Subscribe user to Market Update
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /market-updates
        ///     {   
        ///         DatafeedAction
        ///         [
        ///             "marketIds": 5,
        ///             "sessionId":"string"
        ///         ]
        ///     }
        ///     
        /// </remarks>
        /// <param name="datafeedAction"> Market's id and sessionId</param>
        /// <returns>boolean value</returns>
        /// <response code="200">If successful Subscribed</response>
        /// <response code="400">If not successful Subscribed</response> 
        [HttpPost("subscribe")]
        public  ActionResult<string> Subscribe([FromBody] DatafeedAction datafeedAction)
        {
            if (_generatorService.Subscribe(datafeedAction.MarketId, datafeedAction.SessionId))
            {
                return Ok("OK");
            }
            return Ok("Wrong");
        }

        /// <summary>
        /// Unsubscribe user from Market Update 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /market-updates
        ///     {   
        ///         DatafeedAction
        ///         [
        ///             "marketIds": 5,
        ///             "sessionId":"string"
        ///         ]
        ///     }
        ///     
        /// </remarks>
        /// <param name="datafeedAction"> Market's id and sessionId</param>
        /// <returns>StatusCode</returns>
        /// <response code="200">If successful Ubsubscribed</response>
        /// <response code="400">If not successful Ubsubscribed</response> 
        [HttpPost("unsubscribe")]
        public ActionResult<string> Unsubcribe([FromBody] DatafeedAction datafeedAction)
        {
            if (_generatorService.Unsubcribe(datafeedAction.MarketId, datafeedAction.SessionId))
            {
                return Ok("OK");
            }

            return Ok("Wrong");
        }
        /// <summary>
        /// GET Market Update 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /market-updates/{marketId}
        ///     
        /// </remarks>
        /// <param name="marketId"> Market's Id</param>
        /// <returns>list of market-updates</returns>
        /// <response code="200">If successful return market update</response>
        [HttpGet("{marketId}")]
        public ActionResult<MarketUpdateDto> GetLastUpdateMarketById([FromRoute] long marketId)
        {
            MarketUpdate marketUpdate = new MarketUpdate();
            _generatorService.LastMarketUpdates.TryGetValue(marketId,out marketUpdate);

            return Ok(_mapper.Map<MarketUpdateDto>(marketUpdate));
        }


    }
}
