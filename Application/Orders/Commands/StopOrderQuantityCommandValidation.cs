using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Orders.Commands;

public sealed class StopOrderQuantityCommandValidation : AbstractValidator<StopOrderQuantityCommand>
{
    private readonly IStockUtils _stockUtils;
    private readonly IIdentityService _identityService;

    public StopOrderQuantityCommandValidation(IStockUtils stockUtils , IIdentityService identityService)
    {
        _stockUtils = stockUtils;
        _identityService = identityService;

        RuleFor(x => x.UserId).Cascade(CascadeMode.Stop).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid")
            .MustAsync(UserExists).WithMessage("User not found");

        RuleFor(x => x.StockId).Cascade(CascadeMode.Stop).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid")
            .MustAsync(StockExistInDatabase).WithMessage("Stock not found");

        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.StopPrice).GreaterThan(0);
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
