using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using FluentValidation;

namespace Application.Orders.Commands;

public sealed class MarketOrderAmountCommandValidation : AbstractValidator<MarketOrderAmountCommand>
{
    private readonly IStockUtils _stockUtils;
    private readonly IIdentityService _identityService;

    public MarketOrderAmountCommandValidation(IStockUtils stockUtils, IIdentityService identityService)
    {
        _stockUtils = stockUtils;
        _identityService = identityService;

        RuleFor(x => x.IsBuy).Must(x => x == true)
            .WithMessage("Order Amount only supported for market buy orders");
        
        RuleFor(x => x.UserId).Cascade(CascadeMode.Stop).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid")
            .MustAsync(UserExists).WithMessage("User not found");

        RuleFor(x => x.StockId).Cascade(CascadeMode.Stop).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid")
            .MustAsync(StockExistInDatabase).WithMessage("Stock not found");
        
        RuleFor(x => x.Amount).GreaterThan(0);
    }

    private async Task<bool> StockExistInDatabase(string stockId, CancellationToken cancellationToken)
    {
        return await _stockUtils.ExistsAsync(stockId, cancellationToken);
    }
    private async Task<bool> UserExists(string userId, CancellationToken token)
    {
        return await _identityService.UserExistsAsync(userId, token);
    }
}