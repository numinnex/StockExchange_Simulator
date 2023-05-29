using Application.Common.Models.ReadModels;
using Contracts.V1.Responses;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IStockClient
{
    Task<List<SymbolLookupResponse>> SymbolLookupAsync(string symbol);
    Task<List<Stock>> GetStocksBySymbolAsync(string name );
    Task<decimal> GetRealtimePrice(string symbol);
}