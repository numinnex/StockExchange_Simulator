using FluentValidation;

namespace Application.Auth.Commands;

public sealed class LoginValidator : AbstractValidator<LoginUserCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().NotNull();
    } 
}