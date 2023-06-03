using Application.Common.Interfaces;
using Contracts.V1;
using Domain.Entities;
using Domain.Enums;
using Infrastructure_Tests.Utils.Constants;
using Moq;

namespace Infrastructure_Tests.Utils.Factories;

public static class OrderFactory
{
    public static MarketOrder CreateBuyMarketOrderWithQuantity(int idx, decimal quantity, decimal price,Mock<IDateTimeProvider> dateTimeProvider )
    {
        return new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = OrderConstants.StockId,
            UserId = OrderConstants.UserIdFromIndex(idx),
            Timestamp = dateTimeProvider.Object.Now.AddMicroseconds(idx) ,
            OpenQuantity = quantity,
            OrderAmount = null,
            Price = price,
            FeeId = OrderConstants.FeeId,
            Type = TradeType.BuyMarket,
            TradeCondition = OrderConstants.TradeCondition,
            Symbol = OrderConstants.Symbol,
        };
    }
    public static MarketOrder CreateBuyMarketOrderWithOrderAmount(int idx, decimal orderAmount, decimal price,Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = OrderConstants.StockId,
            UserId = OrderConstants.UserIdFromIndex(idx),
            Timestamp = dateTimeProvider.Object.Now.AddMicroseconds(idx),
            OpenQuantity = null,
            OrderAmount = orderAmount,
            Price = price,
            FeeId = OrderConstants.FeeId,
            Type = TradeType.BuyMarket,
            TradeCondition = OrderConstants.TradeCondition,
            Symbol = OrderConstants.Symbol,
        };
    }
    public static MarketOrder CreateSellMarketOrderWithOrderAmount(int idx, decimal orderAmount, decimal price,
        Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = OrderConstants.StockId,
            UserId = OrderConstants.UserIdFromIndex(idx),
            Timestamp = dateTimeProvider.Object.Now.AddMicroseconds(idx),
            OpenQuantity = null,
            OrderAmount = orderAmount,
            Price = price,
            FeeId = OrderConstants.FeeId,
            Type = TradeType.SellMarket,
            TradeCondition = OrderConstants.TradeCondition,
            Symbol = OrderConstants.Symbol,
        };
    }
    public static MarketOrder CreateSellMarketOrderWithQuantity(int idx, decimal quantity, decimal price,
        Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = OrderConstants.StockId,
            UserId = OrderConstants.UserIdFromIndex(idx),
            Timestamp = dateTimeProvider.Object.Now.AddMicroseconds(idx),
            OpenQuantity = quantity,
            OrderAmount = null,
            Price = price,
            FeeId = OrderConstants.FeeId,
            Type = TradeType.SellMarket,
            TradeCondition = OrderConstants.TradeCondition,
            Symbol = OrderConstants.Symbol,
        };
    }
    public static StopOrder CreateSellStoporder(int idx, decimal quantity, decimal stopPrice,
        Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new StopOrder()
        {
            IsTriggered = false,
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = OrderConstants.StockId,
            UserId = OrderConstants.UserIdFromIndex(idx),
            Timestamp = dateTimeProvider.Object.Now.AddMicroseconds(idx),
            OpenQuantity = quantity,
            StopPrice = stopPrice,
            FeeId = 1,
            Type = TradeType.StopSell,
            Symbol = OrderConstants.Symbol,
        };
    }
    public static StopOrder CreateBuyStopOrder(int idx, decimal quantity, decimal stopPrice,
        Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new StopOrder()
        {
            IsTriggered = false,
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = OrderConstants.StockId,
            UserId = OrderConstants.UserIdFromIndex(idx),
            Timestamp = dateTimeProvider.Object.Now.AddMicroseconds(idx),
            OpenQuantity = quantity,
            StopPrice = stopPrice,
            FeeId = 1,
            Type = TradeType.StopBuy,
            Symbol = OrderConstants.Symbol,
        };
    }
}