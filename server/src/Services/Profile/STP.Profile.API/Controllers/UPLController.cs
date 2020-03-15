using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Profile.Domain.DTO.Order;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.Managers;
using System.Threading.Tasks;
using STP.Profile.UpdateService.Abstract;
using STP.Common.Models;

namespace STP.Profile.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/UPL")]
    [ApiController]
    public class UPLController : Controller
    {
        private IUpdatesManager _updatesManager;

        public UPLController(IUpdatesManager updatesManager)
        {
            _updatesManager = _updatesManager;
        }

        /// <summary>
        /// For internal client only
        /// Subscribing for receiving realtime UPL data for some market
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/UPL/subscribe
        ///     {
        ///         "marketId": 123456
        ///      }
        ///
        /// </remarks>
        /// <response code="200">Returns the array of orders</response>
        [Authorize(Policy = "internal")]
        [HttpPost("subscribe")]
        public async Task<ActionResult> SubscribeAsync(long marketId)
        {
            //  await _updatesManager.SubscribeAsync(marketId);
            return Ok("method muted");
        }

        /// <summary>
        /// For internal client only
        /// UnSubscribing from receiving realtime UPL data for some market
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST  /api/UPL/unsubscribe
        ///     {
        ///         "marketId": 123456
        ///      }
        ///     
        /// </remarks>
        /// <response code="200">If successful returned order</response>
        [Authorize(Policy = "internal")]
        [HttpPost("unsubscribe")]
        public async Task<ActionResult> UnsubscribeAsync(long marketId)
        {
        //    await _updatesManager.UnSubscribeAsync(marketId);
            return Ok("method muted");
        }
    }
}