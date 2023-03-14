using Application.Auth.Commands;
using Application.Common.Interfaces;
using Azure;
using Contracts.V1;
using Contracts.V1.Requests;
using Contracts.V1.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Presentation.Controllers.V1;

[ApiController]
public sealed class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Routes.Identity.Register)]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RegisterUserCommand(request.Email, request.Password)
            , cancellationToken);
        if (!response.Success)
        {
            return BadRequest(new AuthFailResponse()
            {
                Errors = response.Errors!,
            }); 
        }
        return Ok(new AuthSuccesResponse()
        {
           Token = response.Token!,
           RefreshToken = response.RefreshToken!
        }); 
    }

    [HttpPost(Routes.Identity.Login)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new LoginUserCommand(request.Email, request.Password)
            , cancellationToken);
        if (!response.Success)
        {
            return BadRequest(new AuthFailResponse()
            {
                Errors = response.Errors!,
            });
        }
        return Ok(new AuthSuccesResponse()
        {
           Token = response.Token!,
           RefreshToken = response.RefreshToken!
        });
    }

    [HttpPost(Routes.Identity.Refresh)]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
       var response = await _mediator.Send(new RefreshTokenCommand(request.Token, request.RefreshToken)
           , cancellationToken);
       if (!response.Success)
       {
           return BadRequest(new AuthFailResponse()
           {
               Errors = response.Errors!,
           });
       }
       return Ok(new AuthSuccesResponse()
        {
           Token = response.Token!,
           RefreshToken = response.RefreshToken!
        }); 
    }
}