using Application.Common.Intefaces;
using Application.Stocks.Queries;
using Contracts.V1;
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

    [HttpGet(Routes.Stocks.GetRealtimePrice)]
    public async Task<double> GetRealtimePrice([FromRoute] string symbol )
    {
        return await _client.GetRealtimePrice(symbol);
    }
}
