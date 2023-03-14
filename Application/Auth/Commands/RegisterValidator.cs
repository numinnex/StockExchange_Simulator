using FluentValidation;

namespace Application.Auth.Commands;

public sealed class RegisterValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().NotNull()
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            //Special Character
            .Must(x => x.Any(c => !char.IsLetterOrDigit(c)))
            .WithMessage("Your password must contain at least one special character.");
    } 
}
