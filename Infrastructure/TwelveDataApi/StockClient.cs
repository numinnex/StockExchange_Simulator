using System.Net.Http.Json;
using Application.Common.Intefaces;
using Application.Common.Models.ReadModels;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.TwelveDataApi;

public sealed record PriceRead(double price);

public sealed class StockClient : IStockClient
{
    private readonly HttpClient _client;

    public StockClient(HttpClient client)
    {
        _client = client;
    }
    public async Task<List<Stock>> GetStocksBySymbolAsync(string name)
    {
        //TODO - Refactor => move stock price retrieval to a StockPriceService
        
        var stocksResponse = await _client.GetFromJsonAsync<StockResponse>($"stocks?symbol={name}&format=json");
        var stockPriceResponse = await _client.GetAsync($"price?symbol={name}&format=json");
        
        if (stocksResponse!.Status == "ok")
        {
            var stocksResult = new List<Stock>();
            foreach (var stockReadModel in stocksResponse.Data)
            {
                stocksResult.Add(new Stock
                {
                    Name = stockReadModel.Name,
                    Country = stockReadModel.Country,
                    Currency = stockReadModel.Currency,
                    Symbol = stockReadModel.Symbol,
                    Price = stockPriceResponse.IsSuccessStatusCode ? 
                        new Price { Value = (await stockPriceResponse.Content.ReadFromJsonAsync<PriceRead>())!.price }
                        : new Price() { Value = 0.00}
                }); 
            }
            return stocksResult;
        }

        return Enumerable.Empty<Stock>().ToList();
    }
}