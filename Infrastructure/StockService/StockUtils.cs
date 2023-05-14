using System.Data;
using Application.Common.Interfaces;
using Dapper;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.StockService;

public sealed class StockUtils : IStockUtils
{
    private readonly IDbConnection _dbConnection;

    public StockUtils(ApplicationDbContext ctx)
    {
        _dbConnection = ctx.Database.GetDbConnection();
    }
    public async Task<string> GetStockSymbolByStockId(Guid stockId)
    {
        return await _dbConnection.QueryFirstAsync<string>(@"select Symbol from db_stock.Stocks where Id = @stockId",
            new { stockId = stockId });
    }

    public async Task<bool> ExistsAsync(string stockId, CancellationToken token)
    {
        const string query = @"select count(*) from db_stock.Stocks where Id = @stockId";
        return await _dbConnection.QuerySingleAsync<int>(new CommandDefinition(query, cancellationToken: token,
            parameters: new { stockId })) > 0;
    }
}