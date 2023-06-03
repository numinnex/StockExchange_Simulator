using Application.Orders.Commands;
using Contracts.V1;
using Contracts.V1.Requests;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost(Routes.Order.OrderMarketQuantity)]
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
    [HttpPost(Routes.Order.StopOrderQuantity)]
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
    [HttpPost(Routes.Order.OrderMarketAmount)]
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
}