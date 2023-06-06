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
    :IRequest<Result<MarketTradeResponse>>;
public sealed class MarketOrderQuantityHandler : IRequestHandler<MarketOrderQuantityCommand, Result<MarketTradeResponse>>
{
    private readonly IMatchingEngine _matchingEngine;
    private readonly IStockUtils _stockUtils;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IStockClient _stockClient;
    private readonly IPortfolioRepository _portfolioRepository;

    public MarketOrderQuantityHandler(IMatchingEngine matchingEngine, IStockUtils stockUtils,
        IDateTimeProvider dateTimeProvider, IStockClient stockClient, IPortfolioRepository portfolioRepository)
    {
        _matchingEngine = matchingEngine;
        _stockUtils = stockUtils;
        _dateTimeProvider = dateTimeProvider;
        _stockClient = stockClient;
        _portfolioRepository = portfolioRepository;
    }
    public async Task<Result<MarketTradeResponse>> Handle(MarketOrderQuantityCommand request, CancellationToken cancellationToken)
    {
        var stockSymbol = await _stockUtils.GetStockSymbolByStockId(Guid.Parse(request.StockId));
        var realTimePrice = await _stockClient.GetRealtimePrice(stockSymbol);

        var securities = await _portfolioRepository.GetSecuritiesByUserIdAsync(request.UserId, cancellationToken);
        if (!request.IsBuy)
        {
            var security = securities.FirstOrDefault(x => x.StockId == Guid.Parse(request.StockId));
            if (securities.Count == 0 || security is null || security.Quantity < request.Quantity)
            {
                return Result<MarketTradeResponse>.Failure(new[]
                {
                    new Error()
                    {
                        Message = "You don't have enough quantity of this stock",
                        Code = "Not enough quantity"
                    }
                });
            }
        }
        var order = new MarketOrder()
        {
            Id = new Guid(),
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

        return Result<MarketTradeResponse>.Success(new MarketTradeResponse
        {
            StockId = request.StockId,
            Price = decimal.Multiply(request.Quantity , realTimePrice) 
        });
    }
}