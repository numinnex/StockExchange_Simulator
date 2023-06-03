using Application.Common.Interfaces;
using Application.Orders.Commands;
using Application.Trades.Queries;
using Contracts.V1;
using Contracts.V1.Requests;
using Domain.Entities;
using Infrastructure.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUriService _uriService;

    public OrderController(IMediator mediator, IUriService uriService)
    {
        _mediator = mediator;
        _uriService = uriService;
    }
    
    [HttpPost(Routes.Order.PlaceMarketOrderWithQuantity)]
    public async Task<IActionResult> OrderMarketQuantity([FromBody] OrderMarketQuantityTradeRequest request , CancellationToken token)
    {

        var response = await _mediator.Send(new MarketOrderQuantityCommand(request.StockId, request.Quantity
            , request.UserId, request.IsBuy) , token);

        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }
        return BadRequest(response.Errors);
    }
    [HttpPost(Routes.Order.PlaceStopOrderWithQuantity)]
    public async Task<IActionResult> StopOrderQuantity([FromBody]StopOrderQuantityTradeRequest request , CancellationToken token)
    {
        var response = await _mediator.Send(new StopOrderQuantityCommand(
                request.StockId,request.Quantity,request.StopPrice,request.UserId,request.IsBuy
            ) , token);

        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }
        return BadRequest(response.Errors);
    }
    [HttpPost(Routes.Order.PlaceMarketOrderWithAmount)]
    public async Task<IActionResult> OrderMarketAmount([FromBody] OrderMarketAmountTradeRequest request, CancellationToken token)
    {

        var response = await _mediator.Send(new MarketOrderAmountCommand(request.StockId, request.Amount
            , request.UserId, request.IsBuy), token);

        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }
        return BadRequest(response.Errors);
    }
    [HttpGet(Routes.Order.GetActiveTrades)]
    [Authorize]
    public async Task<IActionResult> GetAllActiveTrades([FromQuery]int  pageNumber, [FromQuery] int pageSize)
    {
        var response = await _mediator.Send(new GetActiveMarketOrdersQuery(pageNumber, pageSize));
        if (response.IsSuccess)
        {
            var paginatedResponse = PaginationUtils.CreatePaginatedResponse(_uriService, pageNumber, pageSize, response.Value);
            return Ok(paginatedResponse);
        }

        return BadRequest(response.Errors);
    }
}