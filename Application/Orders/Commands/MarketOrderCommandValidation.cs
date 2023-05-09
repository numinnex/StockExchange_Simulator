using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using FluentValidation;

namespace Application.Orders.Commands;

public sealed class MarketOrderCommandValidation : AbstractValidator<MarketOrderCommand>
{
    private readonly IStockRepository _stockRepository;
    private readonly IIdentityService _identityService;

    public MarketOrderCommandValidation(IStockRepository stockRepository, IIdentityService identityService)
    {
        _stockRepository = stockRepository;
        _identityService = identityService;
    }
    public MarketOrderCommandValidation()
    {
        
        RuleFor(x => x.UserId).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid")
            .MustAsync(UserExists).WithMessage("User not found");
        RuleFor(x => x.StockId).NotEmpty().NotNull().Must(x => Guid.TryParse(x, out var _))
            .WithMessage("Error not valid guid")
            .MustAsync(StockExistInDatabase).WithMessage("Stock not found");
        RuleFor(x => x.Quantity).GreaterThan(0);
    }

    private async Task<bool> StockExistInDatabase(string stockId, CancellationToken cancellationToken)
    {
        return await _stockRepository.ExistAsync(stockId, cancellationToken);
    }
    private async Task<bool> UserExists(string userId, CancellationToken token)
    {
        return await _identityService.UserExistsAsync(userId, token);
    }
}