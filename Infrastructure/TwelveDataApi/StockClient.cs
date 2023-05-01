using System.Net.Http.Json;
using Application.Common.Interfaces;
using Application.Common.Models.ReadModels;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.TwelveDataApi;


public sealed class StockClient : IStockClient
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public StockClient(HttpClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }
    public async Task<List<Stock>> GetStocksBySymbolAsync(string name)
    {
        //TODO - Refactor => Move stock price and stock timeseries to seperate services && move mapping response to domain
        var stocksResponse = await _client
            .GetFromJsonAsync<StockApiResponse>($"stocks?country=United%20States&symbol={name}&format=json");

        if (stocksResponse!.Data.Any())
        {
            var stockPriceResponse = await _client.GetAsync($"price?symbol={name}&format=json");
            var stocksTimeSeriesResponse =
                await _client.GetFromJsonAsync<TimeSeriesResponse>(
                    $"time_series?interval=1day&symbol={name}&format=json&outputsize=730");
            double price = 0.00;

            if (!stockPriceResponse.IsSuccessStatusCode)
            {
                price = 0.00;
            }

            price = (await stockPriceResponse.Content.ReadFromJsonAsync<PriceReadModel>())!.Price;

            var stocksResult = new List<Stock>();

            foreach (var stockReadModel in stocksResponse.Data.ToArray())
            {
                var newStock = MapStockResponse(stockReadModel, price);
                var timeSeries = MapTimeSeriesResponse(stocksTimeSeriesResponse, newStock);
                newStock.TimeSeries = timeSeries;
                stocksResult.Add(newStock);
            }

            return stocksResult;
        }
        return Enumerable.Empty<Stock>().ToList();
    }

    public async Task<double> GetRealtimePrice(string symbol)
    {
        if (_cache.TryGetValue(symbol, out double stockPrice))
        {
            return stockPrice;
        }
        var stockPriceResponse = await _client.GetAsync($"price?symbol={symbol}&format=json");
        if (stockPriceResponse.IsSuccessStatusCode)
        {
            var price = (await stockPriceResponse.Content.ReadFromJsonAsync<PriceReadModel>())!.Price;
            _cache.Set(symbol, price, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            });
            return price;
        }
        return _cache.Get<double>(symbol);
    }

    private static Stock MapStockResponse(StockReadModel stockReadModel, double price)
    {
        var newStock = new Stock
        {
            Name = stockReadModel.Name,
            Country = stockReadModel.Country,
            Currency = stockReadModel.Currency,
            Symbol = stockReadModel.Symbol,
            Price = price
        };
        return newStock;
    }
    private static TimeSeries MapTimeSeriesResponse(TimeSeriesResponse? stocksTimeSeries, Stock newStock)
    {
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
                Volume = x.Volume,
                Open = x.Open,
            }).ToArray(),
            Stock = newStock
        };
        return timeSeriesSnapShot;
    }
}