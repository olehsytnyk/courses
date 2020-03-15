using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Profile.Domain.DTO.Position;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.Managers;
using System.Threading.Tasks;

namespace STP.Profile.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/positions")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private IPositionManager _manager;
        private IMapper _mapper;

        public PositionController(IPositionManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a list of opened (Long and Short) positions by a specific filter. 
        /// Returns first 10 positions by default.
        /// </summary>
        /// <param name="filter">The criteria by which positions are searched. 
        ///     All criteria in the filter are optional.
        /// </param>
        /// <response code="200">Returns the array of positions</response>
        /// <response code="404">If the positions was not found</response>
        [HttpGet("opened")]
        public async Task<ActionResult<GetPositionDTO[]>> GetOpenedAsync([FromQuery] PositionFilterModel filter)
        {
            var entities = await _manager.GetOpenedAsync(filter);
            var dtos = _mapper.Map<PositionEntity[], GetPositionDTO[]>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Get a list of history (Flat) positions by a specific filter. 
        /// Returns first 10 positions by default.
        /// </summary>
        /// <param name="filter">The criteria by which positions are searched. 
        ///     All criteria in the filter are optional.
        /// </param>
        /// <response code="200">Returns the array of positions</response>
        /// <response code="404">If the positions was not found</response>
        [HttpGet("history")]
        public async Task<ActionResult<GetPositionDTO[]>> GetHistoryAsync([FromQuery] PositionFilterModel filter)
        {
            var entities = await _manager.GetHistoryAsync(filter);
            var dtos = _mapper.Map<PositionEntity[], GetPositionDTO[]>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Return position by a certain id 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /positions/{id}
        ///     
        /// </remarks>
        /// <param name="id">position's id</param>
        /// <returns>position model</returns>
        /// <response code="200">If successful returned position</response>
        /// <response code="404">If not found position</response> 
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPositionDTO>> GetAsync([FromRoute] long id)
        {
            var entity = await _manager.FindAsync(id);
            var dto = _mapper.Map<PositionEntity, GetPositionDTO>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Delete position
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /positions/{id}
        ///     
        /// </remarks>
        /// <param name="id"> position's id</param>
        /// <returns>Status code</returns>
        /// <response code="204">If successful deleted position</response>
        /// <response code="404">If not found position</response> 
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _manager.Remove(id);
            return NoContent();
        }
    }
}