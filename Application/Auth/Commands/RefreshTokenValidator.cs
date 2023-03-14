using FluentValidation;

namespace Application.Auth.Commands;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().NotNull();
        RuleFor(x => x.Token).NotEmpty().NotNull();
    } 
}