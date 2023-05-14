using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Contracts.V1.Responses;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Orders.Commands;

public sealed record MarketOrderQuantityCommand(string StockId, decimal Quantity, string UserId, bool IsBuy) 
    :IRequest<Result<MarketOrderResponse>>;
public sealed class MarketOrderQuantityHandler : IRequestHandler<MarketOrderQuantityCommand, Result<MarketOrderResponse>>
{
    private readonly IMatchingEngine _matchingEngine;
    private readonly IStockUtils _stockUtils;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IStockClient _stockClient;

    public MarketOrderQuantityHandler(IMatchingEngine matchingEngine, IStockUtils stockUtils,
        IDateTimeProvider dateTimeProvider, IStockClient stockClient)
    {
        _matchingEngine = matchingEngine;
        _stockUtils = stockUtils;
        _dateTimeProvider = dateTimeProvider;
        _stockClient = stockClient;
    }
    public async Task<Result<MarketOrderResponse>> Handle(MarketOrderQuantityCommand request, CancellationToken cancellationToken)
    {
        var stockSymbol = await _stockUtils.GetStockSymbolByStockId(Guid.Parse(request.StockId));
        var realTimePrice = await _stockClient.GetRealtimePrice(stockSymbol);
        
        var order = new MarketOrder()
        {
            IsBuy = request.IsBuy,
            StockId = Guid.Parse(request.StockId),
            UserId = request.UserId,
            Timestamp = _dateTimeProvider.Now,
            OpenQuantity = request.Quantity,
            OrderAmount = null,
            Price = realTimePrice,
            //FeeId is hardcoded, don't have good idea where it could potentially live
            //maybe based on market?
            FeeId = 1,
            Type = request.IsBuy ? TradeType.BuyMarket : TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = stockSymbol
        };
        await _matchingEngine.AddOrder(order , cancellationToken);

        return Result<MarketOrderResponse>.Success(new MarketOrderResponse
        {
            StockId = request.StockId,
            Price = 12
        });
    }
}