using Application.Common.Interfaces;
using Domain.Auth;
using FluentValidation;
using MediatR;

namespace Application.Auth.Commands;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand , AuthenticationResult>
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserCommandHandler(IIdentityService identityService, IValidator<RegisterUserCommand> validator)
    {
        _identityService = identityService;
        _validator = validator;
    }
    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        
        var validationCtx = new ValidationContext<RegisterUserCommand>(request);
        
        var validationResults = await _validator.ValidateAsync(validationCtx, cancellationToken);

        if (validationResults.Errors.Any())
        {
            return new AuthenticationResult
            {
                Success = false,
                Errors = validationResults.Errors.Select(x => x.ErrorMessage).ToArray()
            };
        }
        
        var authResult  = await _identityService.RegisterAsync(request.Email, request.Password);
        return authResult;
    }
}