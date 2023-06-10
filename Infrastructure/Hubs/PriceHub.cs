using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;
public sealed class PriceHub : Hub
{
    private readonly IStockClient _stockClient;
    public PriceHub(IStockClient stockClient)
    {
        _stockClient = stockClient;
    }
    //the symbol could be passed through request header.
    public async Task<decimal> RealtimePrice(string symbol)
    {
        var price = await _stockClient.GetRealtimePrice(symbol);
        return price;
    }
}