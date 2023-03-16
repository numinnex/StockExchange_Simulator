using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Contracts.V1.Responses;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Infrastructure.TradeServices;

public sealed class TradeService : ITradeService
{
private readonly ITradeRepository _tradeRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TradeService(ITradeRepository tradeRepository, IStockRepository stockRepository, IDateTimeProvider dateTimeProvider)
    {
        _tradeRepository = tradeRepository;
        _stockRepository = stockRepository;
        _dateTimeProvider = dateTimeProvider;
    }
    //TODO - Get the price from the realtime price service rather than stock
    //TODO - Update the trade entity to support trade status
    //TODO - This one is for later, but look into creating some sort of queue for the BuyLimit, SellLimit transactions
    //TODO - Wrap this into transcation
    public async Task<Result<BuyResponse>> BuyMarketAsync(Guid stockId, int quantity, string userId, CancellationToken cancellationToken)
    {
        var stock = await _stockRepository.GetFirstOrDefaultAsync(x => x.Id == stockId, cancellationToken);

        if (stock is null)
        {
            return Result<BuyResponse>.Failure(new List<Error>()
            {
                new Error()
                {
                    Code = "NotFound",
                    Message = "Error Couldn't find Stock",
                }
            });
        }
        var trade = new Trade()
        {
            Quantity = quantity,
            UserId = userId,
            StockId = stockId,
            Timestamp = _dateTimeProvider.Now,
            Price = new Price
            {
                Value = stock.Price.Value * quantity,
            },
            Type = TradeType.BuyMarket,
        };
        await _tradeRepository.CreateBuyTradeAsync(trade, cancellationToken);
        await _tradeRepository.SaveChangesAsync(cancellationToken);
        return Result<BuyResponse>.Success(new BuyResponse()
        {
            Quantity = quantity,
            StockId = stockId.ToString(),
        });
    }
}