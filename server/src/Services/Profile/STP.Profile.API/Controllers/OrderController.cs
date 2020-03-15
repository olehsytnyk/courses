using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STP.Common.Exceptions;
using STP.Profile.Domain.DTO.Order;
using STP.Profile.Domain.Entities;
using STP.Profile.Domain.FilterModels;
using STP.Profile.Interfaces.Managers;
using System.Threading.Tasks;
using System.Security.Claims;
using STP.Interfaces.Enums;


namespace STP.Profile.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/orders")]
    [ApiController]
    public class OrderController : Controller
    {
        private IOrderManager _manager;
        private IMapper _mapper;
        
        public OrderController(IOrderManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a list of orders by a specific filter. 
        /// Returns first 10 orders by default.
        /// </summary>
        /// <param name="filter">The criteria by which orders are searched. 
        ///     All criteria in the filter are optional.
        /// </param>
        /// <response code="200">Returns the array of orders</response>
        /// <response code="404">If the orders was not found</response>
        [HttpGet]
        public async Task<ActionResult<GetOrderDTO[]>> GetAsync([FromQuery] OrderFilterModel filter)
        {
            var entities = await _manager.GetAsync(filter);
            var dtos = _mapper.Map<OrderEntity[], GetOrderDTO[]>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Return order by a certain id 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /orders/{id}
        ///     
        /// </remarks>
        /// <param name="id">order's id</param>
        /// <returns>order model</returns>
        /// <response code="200">If successful returned order</response>
        /// <response code="404">If not found order</response> 
        [HttpGet("{id}")]
        public async Task<ActionResult<GetOrderDTO>> GetAsync([FromRoute] long id)
        {
            var entity = await _manager.FindAsync(id);
            var dto = _mapper.Map<OrderEntity, GetOrderDTO>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Create new order
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /orders
        ///     {
        ///         "marketId": 123456,
        ///         "quantity": 10,
        ///         "action": 1
        ///     }
        ///     
        /// </remarks>
        /// <param name="orderDTO"> order's model</param>
        /// <returns>Newly created order</returns>
        /// <response code="201">If successful created order</response>
        /// <response code="400">If didn't create order</response> 
        [HttpPost]
        public async Task<ActionResult<GetOrderDTO>> PostAsync([FromBody] PostOrderDTO orderDTO)
        {
            OrderEntity entity = _mapper.Map<PostOrderDTO, OrderEntity>(orderDTO);
            entity.UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (entity.UserId == null)
            {
                throw new InvalidPermissionException(ErrorCode.InvalidUserId);
            }

            var result = await _manager.InsertAsync(entity);
            var dto = _mapper.Map<GetOrderDTO>(result);
            return Ok(dto);
        }
    }
}