﻿
using System.Net.Http.Json;
using Application.Common.Models.ReadModels;

var client = new HttpClient();
var request = new HttpRequestMessage
{
    Method = HttpMethod.Get,
    RequestUri = new Uri("https://twelve-data1.p.rapidapi.com/stocks?country=United%20Kingdom&format=json"),
    Headers =
    {
        { "X-RapidAPI-Key", "cc3a2376aamsh10265419dca0ff4p108edcjsn4f8d4d1df821" },
        { "X-RapidAPI-Host", "twelve-data1.p.rapidapi.com" },
    },
};
var secondRequest = new HttpRequestMessage()
{
    Method = HttpMethod.Get,
    RequestUri =
        new Uri("https://twelve-data1.p.rapidapi.com/time_series?symbol=AMZN&interval=1day&outputsize=30&format=json"),
    Headers =
    {
        { "X-RapidAPI-Key", "cc3a2376aamsh10265419dca0ff4p108edcjsn4f8d4d1df821" },
        { "X-RapidAPI-Host", "twelve-data1.p.rapidapi.com" },
    },
};
using (var response = await client.SendAsync(request))
{
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadFromJsonAsync<StockResponse>();
}

using (var response = await client.SendAsync(secondRequest))
{
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadFromJsonAsync<TimeSeriesResponse>();

    Console.WriteLine();
}