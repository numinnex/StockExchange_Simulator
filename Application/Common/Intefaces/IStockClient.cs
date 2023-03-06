using Application.Common.Models.ReadModels;
using Application.Stocks.Queries;
using Domain.Entities;

namespace Application.Common.Intefaces;

public interface IStockClient
{
    Task<List<Stock>> GetStocksByNameAsync(string name);
}