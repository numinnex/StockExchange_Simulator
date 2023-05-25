using Application.Common.Interfaces;
using Application.Stocks.Queries;
using Contracts.V1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[ApiController]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;
        public StockController(IMediator mediator )
        {
            _mediator = mediator;
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
