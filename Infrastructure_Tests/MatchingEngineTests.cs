using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Domain.PriceLevels;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.MatchingEngine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Infrastructure_Tests;

//TODO - refactor the object creation inside of tests, to some builder/factory methods
//TODO - remove all the _tradeListener and _orderRepository setups
public sealed class MatchingEngineTests
{
    private readonly IMatchingEngine _sut;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory = new ();
    private readonly Mock<IServiceScope> _serviceScope = new();
    private readonly Mock<ITradeListener> _tradeListener = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IFeeProvider> _feeProvider = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new();
    private readonly Queue<IReadOnlyList<PriceLevel>> _stopOrdersQueue = new();
    private readonly IBook _book = new Book();


    public MatchingEngineTests()
    {
         _serviceScopeFactory.Setup(x => x.CreateScope()).Returns(_serviceScope.Object);
         _serviceScope.Setup(x => x.ServiceProvider.GetService(typeof(IOrderRepository)))
             .Returns(_orderRepository.Object);
         _serviceScope.Setup(x => x.ServiceProvider.GetService(typeof(ITradeListener)))
             .Returns(_tradeListener.Object);
         _serviceScope.Setup(x => x.ServiceProvider.GetService(typeof(IFeeProvider)))
             .Returns(_feeProvider.Object);
        
        _sut = new MatchingEngine(_serviceScopeFactory.Object, _book,_stopOrdersQueue);
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
        var result = await _sut.AddOrder(order, default);

        result.FirstValue.IsFilled.Should().BeFalse();
        result.FirstValue.IsMatched.Should().BeFalse();
        _book.CurrentMarketPrice.Should().Be(null);
        _book.BestBidsCount.Should().Be(1);
        _book.BestAsksCount.Should().Be(0);
        _book.ClearBook();
        
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

        result.FirstValue.IsFilled.Should().BeFalse();
        result.FirstValue.IsMatched.Should().BeFalse();
        _book.CurrentMarketPrice.Should().Be(null);
        _book.BestBidsCount.Should().Be(0);
        _book.BestAsksCount.Should().Be(1);
        _book.ClearBook();
        
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


        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BestAsksCount.Should().Be(0);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(0);
        _book.BestAsksCount.Should().Be(0);

        buyResult.FirstValue.IsFilled.Should().BeFalse();
        buyResult.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeTrue();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        _book.ClearBook();
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

        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestAsksCount.Should().Be(1);
        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestAsksCount.Should().Be(1);
        _book.BestBidsCount.Should().Be(0);

        buyResult.FirstValue.IsFilled.Should().BeTrue();
        buyResult.FirstValue.IsMatched.Should().BeTrue();
        sellResult.FirstValue.IsFilled.Should().BeFalse();
        sellResult.FirstValue.IsMatched.Should().BeFalse();
        _book.ClearBook();
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

        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestBidsCount.Should().Be(1);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BestAsksCount.Should().Be(0);


        buyResult.FirstValue.IsFilled.Should().BeFalse();
        buyResult.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeTrue();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        _book.ClearBook();
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

        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestBidsCount.Should().Be(1);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(0);
        _book.BestAsksCount.Should().Be(1);
        

        buyResult.FirstValue.IsFilled.Should().BeFalse();
        buyResult.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeFalse();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        _book.ClearBook();
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

        var buyResult1 = await _sut.AddOrder(buyOrder1, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BidSide.Count().Should().Be(1);
        var buyResult2 = await _sut.AddOrder(buyOrder2, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BidSide.Count().Should().Be(2);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(0);
        _book.BidSide.Count().Should().Be(0);
        _book.BestAsksCount.Should().Be(0);


        buyResult1.FirstValue.IsFilled.Should().BeFalse();
        buyResult1.FirstValue.IsMatched.Should().BeFalse();
        buyResult2.FirstValue.IsFilled.Should().BeFalse();
        buyResult2.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeTrue();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        _book.ClearBook();
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

        var buyResult1 = await _sut.AddOrder(buyOrder1, default);
        _book.BestBidsCount.Should().Be(1);
        var buyResult2 = await _sut.AddOrder(buyOrder2, default);
        _book.BestBidsCount.Should().Be(2);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(0);
        _book.BestAsksCount.Should().Be(0);


        buyResult1.FirstValue.IsFilled.Should().BeFalse();
        buyResult1.FirstValue.IsMatched.Should().BeFalse();
        buyResult2.FirstValue.IsFilled.Should().BeFalse();
        buyResult2.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeTrue();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        _book.ClearBook();
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

        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestBidsCount.Should().Be(1);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BestAsksCount.Should().Be(0);
        var sellResult2 = await _sut.AddOrder(sellOrder2, default);
        _book.BestBidsCount.Should().Be(0);
        _book.BestAsksCount.Should().Be(0);

        buyResult.FirstValue.IsFilled.Should().BeFalse();
        buyResult.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeTrue();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        sellResult2.FirstValue.IsFilled.Should().BeTrue();
        sellResult2.FirstValue.IsMatched.Should().BeTrue();
        _book.ClearBook();
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
        
        
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestAsksCount.Should().Be(1);
        _book.AskLevelsCount.Should().Be(1);
        var sellResult2 =await _sut.AddOrder(sellOrder2, default);
        _book.BestAsksCount.Should().Be(1);
        _book.AskLevelsCount.Should().Be(2);
        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestAsksCount.Should().Be(1);
        _book.AskLevelsCount.Should().Be(1);

        
        sellResult.FirstValue.IsFilled.Should().BeFalse();
        sellResult.FirstValue.IsMatched.Should().BeFalse();
        sellResult2.FirstValue.IsFilled.Should().BeFalse();
        sellResult2.FirstValue.IsMatched.Should().BeFalse();
        buyResult.FirstValue.IsFilled.Should().BeTrue();
        buyResult.FirstValue.IsMatched.Should().BeTrue();
        buyResult.FirstValue.Cost!.Value.Should().Be(800);
        _book.ClearBook();

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
        
        
        var buyResult = await _sut.AddOrder(buyOrder, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BidLevelsCount.Should().Be(1);
        var buyResult2 = await _sut.AddOrder(buyOrder2, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BidLevelsCount.Should().Be(2);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        _book.BestBidsCount.Should().Be(1);
        _book.BidLevelsCount.Should().Be(1);
        _book.BestAsksCount.Should().Be(0);

        
        
        buyResult.FirstValue.IsFilled.Should().BeFalse();
        buyResult.FirstValue.IsMatched.Should().BeFalse();
        buyResult2.FirstValue.IsFilled.Should().BeFalse();
        buyResult2.FirstValue.IsMatched.Should().BeFalse();
        sellResult.FirstValue.IsFilled.Should().BeTrue();
        sellResult.FirstValue.IsMatched.Should().BeTrue();
        sellResult.FirstValue.Cost!.Value.Should().Be(1000);
        _book.ClearBook();
    }

    [Fact]
    public async Task StopOrdersShouldBeTriggeredAfterATradeIsMade()
    {
        var stopOrder = new StopOrder()
        {
            IsTriggered = false,
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(1),
            OpenQuantity = 10,
            StopPrice = 120,
            FeeId = 1,
            Type = TradeType.StopBuy,
            Symbol = "APPL"
        };
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,

        });
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user-4",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(2),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 100,
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
            UserId = "user2",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(3),
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
            UserId = "user-54",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(4),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 120,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        var sellOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(5),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 120,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        var sellOrder3 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user12",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(6),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 120,
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

        var stopOrderResult = await _sut.AddOrder(stopOrder, default);
        _book.BestStopBidsCount.Should().Be(1);
        
        var buyResult = await _sut.AddOrder(buyOrder, default);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        var sellResult3 = await _sut.AddOrder(sellOrder3, default);
        var sellResult2 = await _sut.AddOrder(sellOrder2, default);
        var buyResult2 = await _sut.AddOrder(buyOrder2 , default);
        _book.BestAsksCount.Should().Be(0);
        _book.BestBidsCount.Should().Be(0);

        buyResult2.SecondValue.CostAccumulate.Should().Be(1200);
        buyResult2.SecondValue.FilledCount.Should().Be(1);
        buyResult2.SecondValue.MatchedCount.Should().Be(1);
        _book.ClearBook();

    }

    [Fact]
    public async Task StopOrdersShouldBeTriggeredAfterATradeIsMadeAndNonMatchedTurnedIntoOpenOrders()
    {
         var stopOrder = new StopOrder()
        {
            IsTriggered = false,
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(1),
            OpenQuantity = 10,
            StopPrice = 120,
            FeeId = 1,
            Type = TradeType.StopBuy,
            Symbol = "APPL"
        };
         var stopOrder2 = new StopOrder()
        {
            IsTriggered = false,
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(5),
            OpenQuantity = 5,
            StopPrice = 120,
            FeeId = 1,
            Type = TradeType.StopBuy,
            Symbol = "APPL"
        };
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,

        });
        var buyOrder = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = true,
            StockId = Guid.NewGuid(),
            UserId = "user-4",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(2),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 100,
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
            UserId = "user2",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(3),
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
            UserId = "user-54",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(4),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 120,
            FeeId = 1,
            Type = TradeType.BuyMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        var sellOrder2 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user3",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(5),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 120,
            FeeId = 1,
            Type = TradeType.SellMarket,
            TradeCondition = TradeCondition.None,
            Symbol = "APPL"
        };
        var sellOrder3 = new MarketOrder()
        {
            Id = Guid.NewGuid(),
            IsBuy = false,
            StockId = Guid.NewGuid(),
            UserId = "user12",
            Timestamp = _dateTimeProvider.Object.Now.AddMicroseconds(6),
            OpenQuantity = 10,
            OrderAmount = null,
            Price = 120,
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

        var stopOrderResult = await _sut.AddOrder(stopOrder, default);
        _book.BestStopBidsCount.Should().Be(1);
        var stopOrderResult2 = await _sut.AddOrder(stopOrder2, default);
        _book.BestStopBidsCount.Should().Be(2);
        
        var buyResult = await _sut.AddOrder(buyOrder, default);
        var sellResult = await _sut.AddOrder(sellOrder, default);
        var sellResult3 = await _sut.AddOrder(sellOrder3, default);
        var sellResult2 = await _sut.AddOrder(sellOrder2, default);
        var buyResult2 = await _sut.AddOrder(buyOrder2 , default);
        _book.BestStopBidsCount.Should().Be(0);
        _book.BestBidsCount.Should().Be(1);

        buyResult2.SecondValue.CostAccumulate.Should().Be(1200);
        buyResult2.SecondValue.FilledCount.Should().Be(1);
        buyResult2.SecondValue.MatchedCount.Should().Be(1); 
        _book.ClearBook();
    }
}

    

    
