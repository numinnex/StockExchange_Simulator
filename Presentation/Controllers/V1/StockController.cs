using Application.Common.Interfaces;
using Application.Orders.Commands;
using Application.Stocks.Queries;
using Contracts.V1;
using Contracts.V1.Requests;
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

    [HttpGet(Routes.Stock.GetBySymbol)]
    public async Task<IActionResult> GetBySymbol([FromRoute] string symbol , CancellationToken token)
    {
        var response = await _mediator.Send(new GetStockbyNameQuery(symbol), token);

        if (response.IsSuccess)
            return Ok(response.Value);
        if (response.Errors.First().Code == "Not Found")
            return NotFound(response.Errors);

        return BadRequest(response.Errors);
    }

}
