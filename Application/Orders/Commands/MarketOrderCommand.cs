using Application.Common.Models;
using Contracts.V1.Responses;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Orders.Commands;

public record MarketOrderCommand(string StockId, decimal Quantity, string UserId, bool IsBuy) 
    :IRequest<Result<MarketOrderResponse>>;
public sealed class BuyMarketCommandHandler : IRequestHandler<MarketOrderCommand, Result<MarketOrderResponse>>
{
    private readonly IMatchingEngine _matchingEngine;

    public BuyMarketCommandHandler(IMatchingEngine matchingEngine)
    {
        _matchingEngine = matchingEngine;
    }
    public async Task<Result<MarketOrderResponse>> Handle(MarketOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new MarketOrder()
        {
            IsBuy = request.IsBuy,
            StockId = Guid.Parse(request.StockId),
            UserId = request.UserId,
            Timestamp = DateTime.Now,
            OpenQuantity = request.Quantity,
            OrderAmount = null,
            Price = new Price(12),
            FeeId = 1,
            Status = TradeStatus.InQueue,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "AAPL",
        };
        await _matchingEngine.AddOrder(order);

        return Result<MarketOrderResponse>.Success(new MarketOrderResponse
        {
            StockId = request.StockId,
            Quantity = request.Quantity,
            Price = 12
        });
    }
}