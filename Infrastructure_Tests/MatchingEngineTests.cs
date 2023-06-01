using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Domain.PriceLevels;
using FluentAssertions;
using Infrastructure_Tests.Utils.Factories;
using Infrastructure.MatchingEngine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Infrastructure_Tests;

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
        var order = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            12,
            69,
            _dateTimeProvider
        );
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
        var order = OrderFactory.CreateSellMarketOrderWithQuantity(1,
            12,
            69,
            _dateTimeProvider
        );
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
         
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            10,
            70,
            _dateTimeProvider
        );
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(2,
            10,
            70,
            _dateTimeProvider
        ); 
        
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithOrderAmount(
            1,
            300,
            70,
            _dateTimeProvider
        );
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(
            2,
            10,
            70,
            _dateTimeProvider);
       
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(
            1,
            12,
            70,
            _dateTimeProvider);
     
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(2,
            10,
            70,
            _dateTimeProvider);
        
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            5,
            70,
            _dateTimeProvider);
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(2,
            10,70,_dateTimeProvider);
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var buyOrder1 = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            10,
            70,
            _dateTimeProvider);
        var buyOrder2 = OrderFactory.CreateBuyMarketOrderWithQuantity(2,
            5,
            75,
            _dateTimeProvider);

        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(3,
            15,
            70,
            _dateTimeProvider);

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var buyOrder1 = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            10,
            70,
            _dateTimeProvider);
        var buyOrder2 = OrderFactory.CreateBuyMarketOrderWithQuantity(2,
            5,
            70,
            _dateTimeProvider);

        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(3,
            15,
            70,
            _dateTimeProvider);

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            15,
            70,
            _dateTimeProvider);
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(2,
            5,
            70,
            _dateTimeProvider);
        var sellOrder2 = OrderFactory.CreateSellMarketOrderWithQuantity(3,
            10,
            70,
            _dateTimeProvider);

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(1,
            10,
            80,
            _dateTimeProvider);
        var sellOrder2 = OrderFactory.CreateSellMarketOrderWithQuantity(2,
            10,
            100,
            _dateTimeProvider);
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(3,
            10,
            100,
            _dateTimeProvider);
        
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });
        
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
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(1,
            10,
            100,
            _dateTimeProvider);
        var buyOrder2 = OrderFactory.CreateBuyMarketOrderWithQuantity(2,
            10,
            80,
            _dateTimeProvider);
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(3,
            10,
            80,
            _dateTimeProvider);
        
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,

        });
        var stopOrder = OrderFactory.CreateBuyStopOrder(1,
            10,
            120,
            _dateTimeProvider);
        var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(2,
            10,
            100,
            _dateTimeProvider);
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(3,
            10,
            100,
            _dateTimeProvider);
        var buyOrder2 = OrderFactory.CreateBuyMarketOrderWithQuantity(4,
            10,
            120,
            _dateTimeProvider);
        var sellOrder2 = OrderFactory.CreateSellMarketOrderWithQuantity(5,
            10,
            120,
            _dateTimeProvider);
        var sellOrder3 = OrderFactory.CreateSellMarketOrderWithQuantity(6,
            10,
            120,
            _dateTimeProvider); 


        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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
        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });
        var stopOrder = OrderFactory.CreateBuyStopOrder(1,
            10,
            120,
            _dateTimeProvider);
         var stopOrder2 = OrderFactory.CreateBuyStopOrder(2,
            5,
            120,
            _dateTimeProvider);
         var buyOrder = OrderFactory.CreateBuyMarketOrderWithQuantity(3,
            10,
            100,
            _dateTimeProvider);
        var sellOrder = OrderFactory.CreateSellMarketOrderWithQuantity(4,
            10,
            100,
            _dateTimeProvider);
        var buyOrder2 = OrderFactory.CreateBuyMarketOrderWithQuantity(5,
            10,
            120,
            _dateTimeProvider);
        var sellOrder2 = OrderFactory.CreateSellMarketOrderWithQuantity(6,
            10,
            120,
            _dateTimeProvider);
        var sellOrder3 = OrderFactory.CreateSellMarketOrderWithQuantity(7,
            10,
            120,
            _dateTimeProvider);

        _dateTimeProvider.Setup(x => x.Now).Returns(DateTimeOffset.UtcNow);
        _feeProvider.Setup(x => x.GetFeeAsync(1)).ReturnsAsync(new Fee()
        {
            Id = 1,
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });

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

    

    
