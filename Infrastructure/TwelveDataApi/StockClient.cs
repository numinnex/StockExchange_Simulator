using System.Net.Http.Json;
using Application.Common.Intefaces;
using Application.Common.Models.ReadModels;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.TwelveDataApi;


public sealed class StockClient : IStockClient
{
    private readonly HttpClient _client;

    public StockClient(HttpClient client)
    {
        _client = client;
    }
    public async Task<List<Stock>> GetStocksBySymbolAsync(string name)
    {
        //TODO - Refactor => Move stock price and stock timeseries to seperate services 
        
        var stocksResponse = await _client.GetFromJsonAsync<StockResponse>($"stocks?symbol={name}&format=json");
        var stockPriceResponse = await _client.GetAsync($"price?symbol={name}&format=json");
        var stocksTimeSeries =
            await _client.GetFromJsonAsync<TimeSeriesResponse>(
                $"time_series?interval=1day&symbol={name}&format=json&outputsize=30");
        double price = 0.00;

        if (stockPriceResponse.IsSuccessStatusCode)
        {
            price = (await stockPriceResponse.Content.ReadFromJsonAsync<PriceReadModel>())!.Price;
        }

        if (stocksResponse!.Status == "ok")
        {
            var stocksResult = new List<Stock>();
            
            foreach (var stockReadModel in stocksResponse.Data)
            {
                var newStock = new Stock
                {
                    Name = stockReadModel.Name,
                    Country = stockReadModel.Country,
                    Currency = stockReadModel.Currency,
                    Symbol = stockReadModel.Symbol,
                    Price = new Price(){ Value = price},
                };
                var timeSeriesSnapShot = new TimeSeries()
                {
                    Interval = stocksTimeSeries!.Meta.Interval,
                    TimeZone = stocksTimeSeries.Meta.exchange_timezone,
                    StockValues = stocksTimeSeries.Values.Select(x => new StockSnapshot()
                    {
                        Close = x.Close,
                        Datetime = x.DateTime,
                        High = x.High,
                        Low = x.Low,
                        Open = x.Open,
                    }).ToArray(),
                    Stock = newStock
                };
                newStock.TimeSeries = timeSeriesSnapShot;
                stocksResult.Add(newStock);
            }
           
            return stocksResult;
        }

        return Enumerable.Empty<Stock>().ToList();
    }
}