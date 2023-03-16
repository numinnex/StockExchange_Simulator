using Application.Common.Models;
using Contracts.V1.Responses;

namespace Application.Common.Interfaces;

public interface ITradeService
{
    public Task<Result<BuyResponse>> BuyMarketAsync(Guid stockId, int quantity, string userId, CancellationToken cancellationToken);
    public Task<Result<SellResponse>> SellMarketAsync(Guid stockId, int quantity, string userId, CancellationToken cancellationToken);
    //public Task<Result<BuyResponse>> BuyLimitAsync(string stockId, int quantity, string userId, double limitPrice);
}