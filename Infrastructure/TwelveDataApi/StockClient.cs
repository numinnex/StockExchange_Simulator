using Application.Common.Intefaces;
using Domain.Entities;

namespace Infrastructure.TwelveDataApi;

public sealed class StockClient : IStockClient
{
    private readonly HttpClient _client;

    public StockClient(HttpClient client)
    {
        _client = client;
    }
    public Task<List<Stock>> GetStocksByNameAsync(string name)
    {
        throw new NotImplementedException();
    }
}