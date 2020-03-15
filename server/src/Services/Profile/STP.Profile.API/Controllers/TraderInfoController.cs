using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Common.Exceptions;
using STP.Interfaces.Enums;
using STP.Profile.Domain.DTO.TraderInfo;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.Managers;
using System.Threading.Tasks;

namespace STP.Profile.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/traderinfos")]
    [ApiController]
    public class TraderInfoController : ControllerBase
    {
        private ITraderInfoManager _manager;
        private IMapper _mapper;

        public TraderInfoController(ITraderInfoManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a list of Trader Infos. 
        /// Returns first 10 positions by default.
        /// </summary>
        /// <param name="filter">The criteria by which Trader Infos are searched. 
        ///     All criteria in the filter are optional.
        /// </param>
        /// <returns>list of Trader Infos</returns>
        /// <response code="200">Returns the array of Trader Infos</response>
        /// <response code="4406">If the Trader Infos was not found</response>
        [HttpGet("all")]
        public async Task<ActionResult<GetTraderInfoDTO[]>> GetAllAsync([FromQuery] TraderInfoFilterModel filter)
        {
            var entities = await _manager.GetAllAsync(filter);
            var dtos = _mapper.Map<TraderInfoEntity[], GetTraderInfoDTO[]>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Get a list of Trader Infos by a specific filter. 
        /// Returns first 10 positions by default.
        /// </summary>
        /// <param name="filter">The criteria by which Trader Infos are searched. 
        ///     All criteria in the filter are optional.
        /// </param>
        /// <returns>list of Trader Infos</returns>
        /// <response code="200">Returns the array of Trader Infos</response>
        /// <response code="4406">If the Trader Infos was not found</response>
        [HttpGet("filter")]
        public async Task<ActionResult<GetTraderInfoDTO[]>> GetByFilterAsync([FromQuery] TraderInfoFilterModel filter)
        {
            var entities = await _manager.GetByFilterAsync(filter);
            var dtos = _mapper.Map<TraderInfoEntity[], GetTraderInfoDTO[]>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Get a list of Trader Infos in descending order sorted by CopyCount. 
        /// Returns first 10 positions by default.
        /// </summary>
        /// <param name="filter">The criteria by which Trader Infos are searched. 
        ///     All criteria in the filter are optional.
        /// </param>
        /// <returns>list of Trader Infos</returns>
        /// <response code="200">Returns the array of Trader Infos</response>
        /// <response code="4406">If the Trader Infos was not found</response>
        [HttpGet("all/copycount")]
        public async Task<ActionResult<GetTraderInfoDTO[]>> GetByCopyCountAsync([FromQuery] BaseFilterModel filter)
        {
            var entities = await _manager.GetByCopyCountAsync(filter);
            var dtos = _mapper.Map<TraderInfoEntity[], GetTraderInfoDTO[]>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Return Trader Info by a certain id 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /TraderInfos/{id}
        ///     
        /// </remarks>
        /// <param name="id">TraderInfo's id</param>
        /// <returns>trader info model</returns>
        /// <response code="200">If successful returned Trader Info</response>
        /// <response code="4406">If not found Trader Info</response> 
        [HttpGet("{id}")]
        public async Task<ActionResult<GetTraderInfoDTO>> GetAsync([FromRoute] string id)
        {
            var entity = await _manager.GetByIdAsync(id);
            var dto = _mapper.Map<TraderInfoEntity, GetTraderInfoDTO>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Delete Trader Info
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /TraderInfos/{id}
        ///     
        /// </remarks>
        /// <param name="id"> Trader Info's id</param>
        /// <returns>Status code</returns>
        /// <response code="204">If successful deleted Trader Info</response>
        /// <response code="4406">If not found Trader Info</response> 
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            await _manager.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<GetTraderInfoDTO>> UpdateAsync([FromBody] BaseTraderInfoDTO dto)
        {
            var entity = _mapper.Map<TraderInfoEntity>(dto);
            var result = await _manager.UpdateAsync(entity);
            var resultDto = _mapper.Map<GetTraderInfoDTO>(result);
            return Ok(resultDto);
        }

        /// <summary>
        /// Follow trader
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /TraderInfos/{followId}
        ///     
        /// </remarks>
        /// <param name="followId"> Follow Trader Info's id</param>
        /// <returns>Status code</returns>
        /// <response code="204">If successful updates Trader Info</response>
        /// <response code="4406">If not found Follow Trader Info</response> 
        [Authorize]
        [HttpPut("following/{followId}")]
        public async Task<IActionResult> FollowTrader([FromRoute]string followId)
        {
            var followTrader = await _manager.GetByIdAsync(followId);

            if (followTrader == null)
            {
                throw new NotFoundException(ErrorCode.TraderInfoNotFound);
            }

            followTrader.CopyCount++;

            await _manager.UpdateAsync(followTrader);

            return Ok();
        }
    }
}