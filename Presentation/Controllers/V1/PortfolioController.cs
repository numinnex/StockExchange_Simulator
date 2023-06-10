using Application.Portfolios;
using Contracts.V1;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[ApiController]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly IMediator _mediator;

    public PortfolioController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet(Routes.Portfolio.GetUserPortfolio)]
    public async Task<IActionResult> GetPortfolio([FromRoute] string userId)
    {
        var response =await _mediator.Send(new GetUserPortfolioQuery(userId));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }
        return BadRequest(response.Errors);
    }   
    [HttpGet(Routes.Portfolio.GetUserSecurities)]
    public async Task<IActionResult> GetSecurities([FromRoute] string userId)
    {
        var response =await _mediator.Send(new GetUserSecuritiesQuery(userId));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }
        return BadRequest(response.Errors);
    }   
}