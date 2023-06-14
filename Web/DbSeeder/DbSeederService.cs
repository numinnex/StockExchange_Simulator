using System.Runtime.Serialization;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Dapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Identity;
using Domain.ValueObjects;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.DbSeeder;

public sealed class DbSeederService
{
    private readonly IServiceProvider _serviceProvider;
    public const string UserName = "admin@admin.com";
    public const string Password = "pa55w0rd!D";

    public DbSeederService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedDbAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
        var stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();
        var portfolioRepository = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
        var stockClient = scope.ServiceProvider.GetRequiredService<IStockClient>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var userExists = await identityService.FindByEmailAsync(UserName, CancellationToken.None);
        if (!userExists)
        {
            var result = await identityService.RegisterAsync(UserName, Password);
            if (result.Success)
            {
                var user = await dbContext.Users.Where(x => x.UserName == UserName).FirstOrDefaultAsync();
                //Get Apple stock seeded in database
                var stocks = await stockClient.GetStocksBySymbolAsync("AAPL");
                await stockRepository.AddRangeAsync(stocks, default);
                await stockRepository.SaveChangesAsync(default);
                //Create multiple sell orders for the apple stock
                //Add those sell orders into the portfolio of created user
                var portfolio = await portfolioRepository.GetByUserIdAsync(user!.Id, default);
                await CreateOrdersAndSecurities( stocks, user, orderRepository, portfolio!, portfolioRepository ,dbContext);
                Console.WriteLine("FINISHED THE INITIAL SEED");
            }
        }
    }
    private async Task CreateOrdersAndSecurities(List<Stock> stocks, ApplicationUser user, IOrderRepository orderRepository, Portfolio portfolio, IPortfolioRepository portfolioRepository,ApplicationDbContext dbContext)
    {
        var stockId = stocks[0].Id;
        var symbol = stocks[0].Symbol;
        var orders = new List<MarketOrder>();
        var securities = new List<Security>();

        for (int i = 0; i < 4; i++)
        {
            var sellOrder = new MarketOrder
            {
                Price = 69,
                OpenQuantity = (i + 1) * 100,
                UserId = user.Id,
                StockId = stockId,
                Symbol = symbol,
                IsBuy = false,
                Timestamp = DateTimeOffset.UtcNow.AddMinutes(i),
                Type = TradeType.SellMarket,
                FeeId = 1,
                TradeCondition = TradeCondition.None,
                OrderAmount = null,
            };

            orders.Add(sellOrder);

            var security = new Security
            {
                Quantity = sellOrder.OpenQuantity,
                Timestamp = sellOrder.Timestamp.AddSeconds(1),
                PortfolioId = portfolio.Id,
                StockId = stockId, 
                OrderId = sellOrder.Id,
                PurchasedPrice = 69,
            };

            securities.Add(security);
        }

        orderRepository.AddSeedMarketOrdersAsync(orders);
        await orderRepository.SaveChangesAsync(default);

        await using (var connection = dbContext.Database.GetDbConnection())
        {
            connection.Open();
            string query = "INSERT INTO db_stock.Securities (PortfolioId, StockId, OrderId, PurchasedPrice, Timestamp, Quantity_Value) " +
                           "VALUES (@PortfolioId, @StockId, @OrderId, @PurchasedPrice, @Timestamp, @Quantity)";
            
            foreach (var security in securities)
            {
                await connection.ExecuteAsync(query, new
                {
                    Quantity = security.Quantity.Value,
                    Timestamp = security.Timestamp,
                    PortfolioId = security.PortfolioId,
                    StockId = security.StockId,
                    OrderId = security.OrderId,
                    PurchasedPrice = security.PurchasedPrice,
                });
            }
        }
        
    }

}
    
