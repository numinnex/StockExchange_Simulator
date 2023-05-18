using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.MatchingEngine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Infrastructure_Tests;

public sealed class MatchingEngineUnitTests
{
    private readonly IMatchingEngine _sut;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory = new ();
    private readonly Mock<IServiceScope> _serviceScope = new();
    private readonly Mock<ITradeListener> _tradeListener = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IFeeProvider> _feeProvider = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new();
    
    
    public MatchingEngineUnitTests()
    {
         _serviceScopeFactory.Setup(x => x.CreateScope()).Returns(_serviceScope.Object);
         _serviceScope.Setup(x => x.ServiceProvider.GetService(typeof(IOrderRepository)))
             .Returns(_orderRepository.Object);
         _serviceScope.Setup(x => x.ServiceProvider.GetService(typeof(ITradeListener)))
             .Returns(_tradeListener.Object);
         _serviceScope.Setup(x => x.ServiceProvider.GetService(typeof(IFeeProvider)))
             .Returns(_feeProvider.Object);
        
        _sut = new MatchingEngine(_serviceScopeFactory.Object);
    }

    [Fact]
    public async Task BuyMarketOrderShouldAddOrderToBook()
    {
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync( new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });
        IOrder order = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "test-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 12, 
            OrderAmount = null,
            Price = 69,
            FeeId = 1,
            Type = TradeType.BuyMarket ,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(order, default));
        var result = await _sut.AddOrder(order, default);

        result.IsFilled.Should().BeFalse();
        result.IsMatched.Should().BeFalse();
        result.BidLevelsCount.Should().Be(1);
        result.AskLevelsCount.Should().Be(0);
    }
    [Fact]
    public async Task SellMarketOrderShouldAddOrderToBook()
    {
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync( new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });
        IOrder order = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "test-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 12, 
            OrderAmount = null,
            Price = 69,
            FeeId = 1,
            Type = TradeType.BuyMarket ,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(order, default));
        var result = await _sut.AddOrder(order, default);

        result.IsFilled.Should().BeFalse();
        result.IsMatched.Should().BeFalse();
        result.BidLevelsCount.Should().Be(0);
        result.AskLevelsCount.Should().Be(1);
    }

    [Fact]
    public async Task BuyAndSellOrdersShouldGetMatched()
    {
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "test-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder.StockId,
            UserId = "another-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var buyResultTask = _sut.AddOrder(buyOrder, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);

        var results = await Task.WhenAll(buyResultTask, sellResultTask);

        var buyResult = results[0];
        var sellResult = results[1];

        buyResult.IsFilled.Should().BeFalse();
        buyResult.IsMatched.Should().BeFalse();
        buyResult.BidLevelsCount.Should().Be(1);
        buyResult.AskLevelsCount.Should().Be(0);

        sellResult.IsFilled.Should().BeTrue();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(0);
        sellResult.AskLevelsCount.Should().Be(0);
    }
    [Fact]
    public async Task BuyOrdersWithOrderAmountShouldMatchSellOrder()
    {
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "test-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = null,
            OrderAmount = 300,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder.StockId,
            UserId = "another-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var sellResultTask = _sut.AddOrder(sellOrder, default);
        var buyResultTask = _sut.AddOrder(buyOrder, default);

        var results = await Task.WhenAll(buyResultTask, sellResultTask);

        var buyResult = results[0];
        var sellResult = results[1];

        buyResult.IsFilled.Should().BeTrue();
        buyResult.IsMatched.Should().BeTrue();
        buyResult.BidLevelsCount.Should().Be(0);
        buyResult.AskLevelsCount.Should().Be(1);

        sellResult.IsFilled.Should().BeFalse();
        sellResult.IsMatched.Should().BeFalse();
        sellResult.BidLevelsCount.Should().Be(0);
        sellResult.AskLevelsCount.Should().Be(1);
    }

    [Fact]
    public async Task BuyOrderShouldBePartiallyFilled()
    {
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "test-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 12,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder.StockId,
            UserId = "another-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var buyResultTask = _sut.AddOrder(buyOrder, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);

        var results = await Task.WhenAll(buyResultTask, sellResultTask);

        var buyResult = results[0];
        var sellResult = results[1];

        buyResult.IsFilled.Should().BeFalse();
        buyResult.IsMatched.Should().BeFalse();
        buyResult.BidLevelsCount.Should().Be(1);
        buyResult.AskLevelsCount.Should().Be(0);

        sellResult.IsFilled.Should().BeTrue();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(1);
        sellResult.AskLevelsCount.Should().Be(0);
    }
     [Fact]
    public async Task SellOrderShouldBePartiallyFilled()
    {
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "test-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 5,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder.StockId,
            UserId = "another-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var buyResultTask = _sut.AddOrder(buyOrder, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);

        var results = await Task.WhenAll(buyResultTask, sellResultTask);

        var buyResult = results[0];
        var sellResult = results[1];

        buyResult.IsFilled.Should().BeFalse();
        buyResult.IsMatched.Should().BeFalse();
        buyResult.BidLevelsCount.Should().Be(1);
        buyResult.AskLevelsCount.Should().Be(0);

        sellResult.IsFilled.Should().BeFalse();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(0);
        sellResult.AskLevelsCount.Should().Be(1);
    }

    [Fact]
    public async Task MatchingOrdersWithDifferentQuantitiesAndPrices()
    {
        var buyOrder1 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user1",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var buyOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = buyOrder1.StockId,
            UserId = "user2",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 5,
            OrderAmount = null,
            Price = 75,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder1.StockId,
            UserId = "another-user-id",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 15,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var buyResultTask1 = _sut.AddOrder(buyOrder1, default);
        var buyResultTask2 = _sut.AddOrder(buyOrder2, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);

        await Task.WhenAll(buyResultTask1, buyResultTask2, sellResultTask);

        var buyResult1 = await buyResultTask1;
        var buyResult2 = await buyResultTask2;
        var sellResult = await sellResultTask;

        buyResult1.IsFilled.Should().BeFalse();
        buyResult1.IsMatched.Should().BeFalse();
        buyResult1.BidLevelsCount.Should().Be(1);
        buyResult1.AskLevelsCount.Should().Be(0);

        buyResult2.IsFilled.Should().BeFalse();
        buyResult2.IsMatched.Should().BeFalse();
        buyResult2.BidLevelsCount.Should().Be(2);
        buyResult2.AskLevelsCount.Should().Be(0);

        sellResult.IsFilled.Should().BeFalse();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(1);
        sellResult.AskLevelsCount.Should().Be(1);
    }

    [Fact]
    public async Task MatchingOrdersWithSellQuantityEqualToSumOfBuyQuantities()
    {
        var buyOrder1 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user1",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var buyOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = buyOrder1.StockId,
            UserId = "user2",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(20),
            OpenQuantity = 5,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder1.StockId,
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMinutes(2),
            OpenQuantity = 15,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var buyResultTask1 = _sut.AddOrder(buyOrder1, default);
        var buyResultTask2 = _sut.AddOrder(buyOrder2, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);

        var results = await Task.WhenAll(buyResultTask1, buyResultTask2, sellResultTask);

        var buyResult1 = results[0];
        var buyResult2 = results[1];
        var sellResult = results[2];

        buyResult1.IsFilled.Should().BeFalse();
        buyResult1.IsMatched.Should().BeFalse();
        buyResult1.BidLevelsCount.Should().Be(1);
        buyResult1.AskLevelsCount.Should().Be(0);

        buyResult2.IsFilled.Should().BeFalse();
        buyResult2.IsMatched.Should().BeFalse();
        buyResult2.BidLevelsCount.Should().Be(1);
        buyResult2.AskLevelsCount.Should().Be(0);

        sellResult.IsFilled.Should().BeTrue();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(0);
        sellResult.AskLevelsCount.Should().Be(0);
    }
    [Fact]
    public async Task MatchingOrdersWithBuyQuantityEqualToSumOfSellQuantities()
    {
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user1",
            Timestamp = _dateTimeProvider.Object.Now,
            OpenQuantity = 15,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };


        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder.StockId,
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(2),
            OpenQuantity = 5,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        var sellOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = buyOrder.StockId,
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(2),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 70,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));

        var buyResultTask1 = _sut.AddOrder(buyOrder, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);
        var sellResultTask2 = _sut.AddOrder(sellOrder2, default);

        var results = await Task.WhenAll(buyResultTask1, sellResultTask, sellResultTask2);

        var buyResult = results[0];
        var sellResult = results[1];
        var sellResult2 = results[2];

        buyResult.IsFilled.Should().BeFalse();
        buyResult.IsMatched.Should().BeFalse();
        buyResult.BidLevelsCount.Should().Be(1);
        buyResult.AskLevelsCount.Should().Be(0);


        sellResult.IsFilled.Should().BeTrue();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(1);
        sellResult.AskLevelsCount.Should().Be(0);
        
        sellResult2.IsFilled.Should().BeTrue();
        sellResult2.IsMatched.Should().BeTrue();
        sellResult2.BidLevelsCount.Should().Be(0);
        sellResult2.AskLevelsCount.Should().Be(0);
    }

    [Fact]
    public async Task MatchingSellOrdersWithBuyOrdersShouldChoseLowerPrice()
    {
        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(2),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 80,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        }; 
        var sellOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user2",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(4),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 100,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        }; 
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(12),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 100,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        }; 
        
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));
        
        
        var sellResultTask = _sut.AddOrder(sellOrder, default);
        var sellResultTask2 = _sut.AddOrder(sellOrder2, default);
        var buyResultTask1 = _sut.AddOrder(buyOrder, default);

        var resutls = await Task.WhenAll(sellResultTask, sellResultTask2, buyResultTask1);
        var sellResult = resutls[0];
        var sellResult2 = resutls[1];
        var buyResult = resutls[2];
        
        
        sellResult.IsFilled.Should().BeFalse();
        sellResult.IsMatched.Should().BeFalse();
        sellResult.BidLevelsCount.Should().Be(0);
        sellResult.AskLevelsCount.Should().Be(1);
        
        sellResult2.IsFilled.Should().BeFalse();
        sellResult2.IsMatched.Should().BeFalse();
        sellResult2.BidLevelsCount.Should().Be(0);
        sellResult2.AskLevelsCount.Should().Be(2);
        
        buyResult.IsFilled.Should().BeTrue();
        buyResult.IsMatched.Should().BeTrue();
        buyResult.BidLevelsCount.Should().Be(0);
        buyResult.AskLevelsCount.Should().Be(1);
        buyResult.Cost!.Value.Should().Be(800);

    }
     [Fact]
    public async Task MatchingBuyOrdersWithSellOrdersShouldChoseHigherPrice()
    {
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(2),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 100,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        }; 
        var buyOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user2",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(4),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 80,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        }; 
        var sellOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(12),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 80,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        }; 
        
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

        _tradeListener.Setup(x => x.OnTradeAsync(It.IsAny<TradeFootprint>(), default));
        _tradeListener.Setup(x => x.OnAcceptAsync(It.IsAny<IOrder>(), default));
        
        
        var buyResultTask = _sut.AddOrder(buyOrder, default);
        var buyResultTask2 = _sut.AddOrder(buyOrder2, default);
        var sellResultTask = _sut.AddOrder(sellOrder, default);

        var resutls = await Task.WhenAll(buyResultTask, buyResultTask2, sellResultTask);
        var buyResult = resutls[0];
        var buyResult2 = resutls[1];
        var sellResult = resutls[2];
        
        
        buyResult.IsFilled.Should().BeFalse();
        buyResult.IsMatched.Should().BeFalse();
        buyResult.BidLevelsCount.Should().Be(1);
        buyResult.AskLevelsCount.Should().Be(0);

        buyResult2.IsFilled.Should().BeFalse();
        buyResult2.IsMatched.Should().BeFalse();
        buyResult2.BidLevelsCount.Should().Be(2);
        buyResult2.AskLevelsCount.Should().Be(0);
        
        sellResult.IsFilled.Should().BeTrue();
        sellResult.IsMatched.Should().BeTrue();
        sellResult.BidLevelsCount.Should().Be(1);
        sellResult.AskLevelsCount.Should().Be(0);
        sellResult.Cost!.Value.Should().Be(1000);
    }
    
    
}

    

    
