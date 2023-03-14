using Application.Common.Interfaces;
using Domain.Auth;
using FluentValidation;
using MediatR;

namespace Application.Auth.Commands;

public sealed record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthenticationResult>;

//TODO - Look into refactoring AuthenticationResult into a Result<Authentication>

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<RefreshTokenCommand> _validator;

    public RefreshTokenCommandHandler(IIdentityService identityService, IValidator<RefreshTokenCommand> validator)
    {
        _identityService = identityService;
        _validator = validator;
    }
    public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validationCtx = new ValidationContext<RefreshTokenCommand>(request);
        
        var validationResults = await _validator.ValidateAsync(validationCtx, cancellationToken);

        if (validationResults.Errors.Any())
        {
            return new AuthenticationResult
            {
                Success = false,
                Errors = validationResults.Errors.Select(x => x.ErrorMessage).ToArray()
            };
        }
        var response = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
        return response;
    }
}