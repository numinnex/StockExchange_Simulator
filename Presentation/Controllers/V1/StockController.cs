using Application.Stocks.Queries;
using Contracts.V1;
using Contracts.V1.Requests;
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
        [Authorize]
        public async Task<IActionResult> GetBySymbol([FromRoute] string symbol , CancellationToken token)
        {
            var response = await _mediator.Send(new GetStockbyNameQuery(symbol), token);

            if (response.IsSuccess)
                return Ok(response.Value);
            if (response.Errors.First().Code == "Not Found")
                return NotFound(response.Errors);

        return BadRequest(response.Errors);
        }

        [HttpPost(Routes.Stock.SymbolLookup)]
        [Authorize]
        public async Task<IActionResult> SymbolLookup([FromBody]SymbolLookupRequest request , CancellationToken token)
        {
            var response = await _mediator.Send(new SymbolLookupQuery(request.Symbol));

            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(response.Errors);
        }
        

}
