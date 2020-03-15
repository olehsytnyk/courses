using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using STP.Common.Exceptions;
using STP.Interfaces.Enums;
using STP.Markets.Application;
using STP.Markets.Application.Enums;
using STP.Markets.Domain.Entities;
using STP.Markets.WatchlistManagerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using STP.Interfaces.Messages;
using STP.Interfaces.Enums;

[Authorize]
[Route("api/watchlists")]
[ApiController]
[Produces("application/json")]
public class WatchlistsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IWatchlistManager _manager;
    private readonly IValidator<WatchlistPostDto> _validator;
    private readonly IMessageBus _messageBus;
    public WatchlistsController(IMapper mapper, IWatchlistManager manager, IValidator<WatchlistPostDto> validator, IMessageBus messageBus)
    {
        _mapper = mapper;
        _manager = manager;
        _validator = validator;
        _messageBus = messageBus;
    }

    /// <summary>
    /// Get watchlists that match filter. If filter's field is empty it will not be applied.
    /// </summary>
    /// <returns>A watchlists on success. 3404 status code if any watchlists do not match filter.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WatchlistDto>>> GetAsync([FromQuery]WatchlistFilterDto filter, int skip, int take)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new InvalidPermissionException(ErrorCode.InvalidUserId);
        }

        Expression<Func<Watchlist, bool>> expression = w =>
            (string.IsNullOrEmpty(filter.Name) || w.Name.Contains(filter.Name)) &&
            (string.IsNullOrEmpty(userId) || w.UserId == userId);

        var watchlists = await _manager.GetWatchlistsAsync(expression, skip, take);

        var watchlistDtos = _mapper.Map<IEnumerable<Watchlist>, IEnumerable<WatchlistDto>>(watchlists);

        return watchlistDtos.ToList();
    }

    /// <summary>
    /// Get watchlist by id.
    /// </summary>
    /// <returns>A watchlist on success. 3404 status code if watchlist with given id does not exist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<WatchlistDto>> GetById(long id)
    {
        Watchlist watchlist = await _manager.GetByIdAsync(id);

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidPermissionException(ErrorCode.InvalidUserId);
        if (watchlist.UserId != userId)
            throw new InvalidPermissionException(ErrorCode.InvalidPermissionMarket);

        return _mapper.Map<WatchlistDto>(watchlist);
    }

    /// <summary>
    /// Create new watchlist in database.
    /// </summary>
    /// <returns>204 status code on success. 400 status code if Name property is empty.</returns>
    [HttpPost]
    public async Task<ActionResult<WatchlistDto>> Post(WatchlistPostDto item)
    {
        if (!_validator.Validate(item).IsValid)
            return BadRequest();

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Watchlist watchlist = _mapper.Map<WatchlistPostDto, Watchlist>(item);

        watchlist.UserId = userId ?? throw new InvalidPermissionException(ErrorCode.InvalidUserId);

        var entity = await _manager.CreateAsync(watchlist);
        WatchlistDto dto = _mapper.Map<WatchlistDto>(entity);
        WatchlistDtoI dtoI = _mapper.Map<WatchlistDtoI>(dto);
        _messageBus.Publish(dtoI, "exc.Market", RabbitExchangeType.DirectExchange, $"Add:{userId}");
        return Ok(dto);
    }

    /// <summary>
    /// Delete watchlist with given id from database.
    /// </summary>
    /// <returns>204 status code on success. 3404 status code if watchlist with given id does not exist.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        Watchlist watchlist = await _manager.GetByIdAsync(id);
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidPermissionException(ErrorCode.InvalidUserId);
        if (watchlist.UserId != userId)
            throw new InvalidPermissionException(ErrorCode.InvalidPermissionMarket);
        await _manager.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Edit watchlist with given id
    /// </summary>
    /// <remarks>You can only change name with this method. To add/remove markets from wathclist use Put method</remarks>
    /// <returns>200 status code on success.</returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult<WatchlistDto>> Patch(long id, [FromBody]JsonPatchDocument<WatchlistPatchDto> watchlistPatch)
    {
        Watchlist watchlist = await _manager.GetByIdAsync(id);

        var watchlistDto = _mapper.Map<WatchlistPatchDto>(watchlist);

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidPermissionException(ErrorCode.InvalidUserId);
        if (watchlist.UserId != userId)
            throw new InvalidPermissionException(ErrorCode.InvalidPermissionMarket);

        watchlistPatch.ApplyTo(watchlistDto);

        watchlist = _mapper.Map<WatchlistPatchDto, Watchlist>(watchlistDto,
            opt => opt.AfterMap((src, dest) =>
            {
                dest.Id = watchlist.Id;
                dest.UserId = watchlist.UserId;
                dest.MarketWatchlists = watchlist.MarketWatchlists;
            }));

        var entity = await _manager.UpdateAsync(watchlist);
        var dto = _mapper.Map<WatchlistDto>(entity);
        WatchlistDtoI dtoI = _mapper.Map<WatchlistDtoI>(dto);
        _messageBus.Publish(dtoI, "exc.Market", RabbitExchangeType.DirectExchange, $"Update:{userId}");
        return Ok(dto);
    }

    /// <summary>
    ///     Add or remove item form Markets collection
    /// </summary>
    /// <param name="id">id of watchlist</param>
    /// <param name="watchlistDtos">Objects with properties Action "Add"/"Remove" (0/1) and Id of market in Markets collection</param>
    /// <returns>200 status code on success.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(long id, IEnumerable<WatchlistPutDto> watchlistDtos)
    {
        Watchlist watchlist = await _manager.GetByIdAsync(id);

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidPermissionException(ErrorCode.InvalidUserId);
        if (watchlist.UserId != userId)
            throw new InvalidPermissionException(ErrorCode.InvalidPermissionMarket);

        var watchlistMarketsToRemove = new List<MarketWatchlist>();

        foreach (var watchlistDto in watchlistDtos)
        {
            bool contains = false;
            MarketWatchlist marketWatchlist = null;
            foreach (var mw in watchlist.MarketWatchlists)
                if (watchlistDto.Id == mw.MarketId)
                {
                    contains = true;
                    marketWatchlist = mw;
                    break;
                }

            switch (watchlistDto.Action)
            {
                case PutAction.Add:
                    if (contains)
                        throw new InvalidDataException(ErrorCode.InvalidMarket, "Market already in the list");
                    watchlist.MarketWatchlists.Add(new MarketWatchlist { MarketId = watchlistDto.Id });
                    //WatchlistDtoI dtoI = _mapper.Map<WatchlistDtoI>(dto);
                    //_messageBus.Publish(dtoI, "exc.Market", RabbitExchangeType.DirectExchange, $"Add:{userId}");

                    break;
                case PutAction.Remove:
                    if (!contains)
                        throw new NotFoundException(ErrorCode.MarketNotFount);
                    watchlist.MarketWatchlists.Remove(marketWatchlist);
                    watchlistMarketsToRemove.Add(marketWatchlist);
                    //WatchlistDtoI dtoI = _mapper.Map<WatchlistDtoI>(dto);
                    //_messageBus.Publish(dtoI, "exc.Market", RabbitExchangeType.DirectExchange, $"Delete:{userId}");
                    break;
                default:
                    throw new InvalidDataException(ErrorCode.InvalidMarket);
            }
        }

        await _manager.RemoveMarketsAsync(watchlistMarketsToRemove);
        await _manager.UpdateAsync(watchlist);

        return Ok();
    }
}