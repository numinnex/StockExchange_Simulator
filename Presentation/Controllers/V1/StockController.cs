using Application.Common.Interfaces;
using Application.Stocks.Queries;
using Application.Trades.Commands;
using Contracts.V1;
using Contracts.V1.Requests;
using Contracts.V1.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[ApiController]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStockClient _client;

    public StockController(IMediator mediator, IStockClient client)
    {
        _mediator = mediator;
        _client = client;
    }

    [HttpGet(Routes.Stocks.GetBySymbol)]
    public async Task<IActionResult> GetBySymbol([FromRoute] string symbol)
    {
        var response = await _mediator.Send(new GetStockbyNameQuery(symbol));
        
        if(response.IsSuccess)
            return Ok(response.Value);
        if (response.Errors.First().Code == "Not Found")
            return NotFound(response.Errors);

        return BadRequest(response.Errors);
    }

    [HttpPost(Routes.Stocks.BuyMarket)]
    public async Task<IActionResult> BuyMarket([FromBody] BuyMarketRequest request)
    {
        var response = await _mediator.Send(new BuyMarketCommand(request.StockId , request.Quantity 
        , request.UserId));

        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }
        return BadRequest(response.Errors); 
    }
}
