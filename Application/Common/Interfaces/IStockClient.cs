using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IStockClient
{
    Task<List<Stock>> GetStocksBySymbolAsync(string name);
    Task<decimal> GetRealtimePrice(string symbol);
}