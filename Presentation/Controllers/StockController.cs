using Application.Stocks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet()]
    public async Task<IActionResult> GetByName(string name)
    {
        var response = await _mediator.Send(new GetStockbyNameQuery() {Symbol = name});
        
        if(response.IsSuccess)
            return Ok(response.Value);

        return BadRequest(response.Errors);
    }
}
