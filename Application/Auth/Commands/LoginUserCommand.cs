using Application.Common.Interfaces;
using Domain.Auth;
using FluentValidation;
using MediatR;

namespace Application.Auth.Commands;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationResult> 
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<LoginUserCommand> _validator;

    public LoginUserCommandHandler(IIdentityService identityService , IValidator<LoginUserCommand> validator)
    {
        _identityService = identityService;
        _validator = validator;
    }
    
    public async Task<AuthenticationResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var validationCtx = new ValidationContext<LoginUserCommand>(request);
        
        var validationResults = await _validator.ValidateAsync(validationCtx, cancellationToken);

        if (validationResults.Errors.Any())
        {
            return new AuthenticationResult
            {
                Success = false,
                Errors = validationResults.Errors.Select(x => x.ErrorMessage).ToArray()
            };
        }
        
        var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
        return authResponse;
    }
}