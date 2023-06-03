using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Repository;
using Xunit;

namespace Infrastructure_Tests.IntegrationTests;

[Collection("db-fixture")]
public sealed class DatabaseIntegrationsTests : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;
    
    private readonly ApplicationDbContext _ctx;
    private readonly IOrderRepository _orderRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IFeeRepository _feeRepository;
    private string UserId = Guid.NewGuid().ToString();
    private Guid OrderId = Guid.NewGuid();  
    public DatabaseIntegrationsTests(DbContextFixture dbContextFixture)
    {
        _ctx = dbContextFixture.ctx;
        _resetDatabase = dbContextFixture.ResetDatabaseAsync;
        _orderRepository = new OrderRepository(_ctx);
        _stockRepository = new StockRepository(_ctx);
        _feeRepository = new FeeRepository(_ctx);
    }

    [Fact]
    public async Task AddingFeeToDatabase()
    {
        var fee = new Fee()
        {
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        };
        await _feeRepository.AddAsync(fee, default);
        await _feeRepository.SaveChangesAsync(default);
        var result = await _feeRepository.ExistAsync(fee.Id, default);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddingAStockToDatabase()
    {
        var stock = new Stock()
        {
            Symbol = "APPL",
            Country = "USA",
            Currency = "USD",
            Name = "Apple Inc.",
            Price = 100,
            Volume = 100,
            HighMonth = 100,
            LowMonth = 100,
            TimeSeries = new TimeSeries()
            {
                Interval = "1day",
                StockValues = new List<StockSnapshot>(),
            }
        };
        await _stockRepository.AddAsync(stock, default);
        await _stockRepository.SaveChangesAsync(default);

        var getStock = await _stockRepository.GetAllAsync(default);
        getStock[0].Id.Should().Be(stock.Id);
        getStock[0].Symbol.Should().Be("APPL");
        getStock.Should().NotBeEmpty();
    }
    

    [Fact]
    public async Task AddingOrderToDatabase()
    {
        await _ctx.Users.AddAsync(new ApplicationUser()
        {
            Id = UserId,
            Email = "VuGkE@example.com",
            UserName = "VuGkE@example.com",
        });
        await _ctx.Fees.AddAsync(new Fee()
        {
            MakerFee = 0.25m,
            TakerFee = 0.1m,
        });
        var stock = new Stock()
        {
            Symbol = "APPL",
            Country = "USA",
            Currency = "USD",
            Name = "Apple Inc.",
            Price = 100,
            Volume = 100,
            HighMonth = 100,
            LowMonth = 100,
            TimeSeries = new TimeSeries()
            {
                Interval = "1day",
                StockValues = new List<StockSnapshot>(),
            }
        };
        await _stockRepository.AddAsync(stock, default);
        await _stockRepository.SaveChangesAsync(default);
        
        var mOrder = new MarketOrder()
        {
            Id = OrderId,
            UserId = UserId,
            StockId = stock.Id,
            Price = 100,
            Symbol = "APPL",
            Timestamp = DateTime.Now,
            Type = TradeType.BuyMarket,
            FeeId = 1,
            IsBuy = true,
            OpenQuantity = 10,
            OrderAmount = null,
            TradeCondition = TradeCondition.None
        };
        await _orderRepository.AddAsync(mOrder, default);
        await _orderRepository.SaveChangesAsync(default);

        var getOrder = await _orderRepository.GetAllMarketOrdersAsync(default);
        getOrder[0].Id.Should().Be(mOrder.Id);
        getOrder.Should().NotBeEmpty();
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}