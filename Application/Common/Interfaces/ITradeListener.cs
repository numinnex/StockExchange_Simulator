using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITradeListener
{
    Task OnTradeAsync(TradeFootprint tradeFootprint, CancellationToken token);
    Task OnAcceptAsync(IOrder order, CancellationToken token);
}