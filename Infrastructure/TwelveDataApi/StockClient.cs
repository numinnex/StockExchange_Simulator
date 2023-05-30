using System.Net.Http.Json;
using Application.Common.Interfaces;
using Application.Common.Models.ReadModels;
using Contracts.V1.Responses;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using TimeSeriesResponse = Application.Common.Models.ReadModels.TimeSeriesResponse;

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


    public async Task<List<SymbolLookupResponse>> SymbolLookupAsync(string symbol)
    {
        var response = await _client.GetFromJsonAsync<SymbolLookupApiResponse>(
            $"symbol_search?outputsize=120&symbol={symbol}");
        if (response!.Data.Any())
        {
            var responseMapped = response.Data.Where(
                    x => x.Country == "United States" && x.Exchange == "NASDAQ"
                    && !string.IsNullOrEmpty(x.Instrument_Name))
                .Take(15)
                .Select(x => new SymbolLookupResponse()
                {
                    Symbol = x.Symbol,
                    Description = x.Instrument_Name
                }).ToList();
            return responseMapped;
        }
        return Enumerable.Empty<SymbolLookupResponse>().ToList();
    }

    public async Task<List<Stock>> GetStocksBySymbolAsync(string name )
    {
        var stocksResponse = await _client
            .GetFromJsonAsync<StockApiResponse>($"stocks?country=United%20States&exchange=NASDAQ&symbol={name}&format=json");

        //minmax?symbol=AAPL&interval=1month&time_period=30&format=json&outputsize=1&series_type=high


        if (stocksResponse!.Data.Any())
        {
            var openHiLow = await _client
                .GetFromJsonAsync<StockHiLoResponse>(
                    $"minmax?symbol={name}&interval=1month&time_period=30&format=json&outputsize=1&series_type=open");
        
            var stockPriceResponse = await _client.GetAsync($"price?symbol={name}&format=json");
            var stocksTimeSeriesResponse =
                await _client.GetFromJsonAsync<TimeSeriesResponse>(
                    $"time_series?interval=1day&symbol={name}&format=json&outputsize=730");
            decimal price = 0.00m;

            if (!stockPriceResponse.IsSuccessStatusCode)
            {
                price = 0.00m;
            }

            price = (await stockPriceResponse.Content.ReadFromJsonAsync<PriceReadModel>())!.Price;

            var stocksResult = new List<Stock>();

            foreach (var stockReadModel in stocksResponse.Data.ToArray())
            {
                var newStock = MapStockResponse(stockReadModel, price , openHiLow!);
                var timeSeries = MapTimeSeriesResponse(stocksTimeSeriesResponse, newStock);
                newStock.TimeSeries = timeSeries;
                stocksResult.Add(newStock);
            }

            return stocksResult;
        }
        return Enumerable.Empty<Stock>().ToList();
    }

    public async Task<decimal> GetRealtimePrice(string symbol)
    {
        if (_cache.TryGetValue(symbol, out decimal stockPrice))
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
        return _cache.Get<decimal>(symbol);
    }

    private static Stock MapStockResponse(StockReadModel stockReadModel, decimal price, StockHiLoResponse openHiLo)
    {
        var newStock = new Stock
        {
            Volume = 0,
            HighMonth = openHiLo.values.First().max,
            LowMonth = openHiLo.values.First().min,
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