using FluentValidation;

namespace Application.Trades.Commands;

public sealed class BuyMarketCommandValidator : AbstractValidator<MarketOrderCommand>
{
    public BuyMarketCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid");
        RuleFor(x => x.StockId).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid");
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}