using Application.Common.Models.ReadModels;
using Application.Stocks.Queries;
using Domain.Entities;

namespace Application.Common.Intefaces;

public interface IStockClient
{
    Task<List<Stock>> GetStocksBySymbolAsync(string name);
    Task<double> GetRealtimePrice(string symbol );
}