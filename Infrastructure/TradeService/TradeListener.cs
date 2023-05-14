using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Entities;

namespace Infrastructure.TradeService;

public sealed class TradeListener : ITradeListener
{
    //TODO - Add logging in there aswell
    private readonly ITradeRepository _tradeRepository;
    private readonly IOrderRepository _orderRepository;

    public TradeListener(ITradeRepository tradeRepository, IOrderRepository orderRepository)
    {
        _tradeRepository = tradeRepository;
        _orderRepository = orderRepository;
    }

    public async Task OnTradeAsync(TradeFootprint tradeFootprint, CancellationToken token)
    {
        await _tradeRepository.AddTradeFootprintAsync(tradeFootprint, token);
        await _tradeRepository.SaveChangesAsync(token);
    }

    public async Task OnAcceptAsync(IOrder order, CancellationToken token)
    {
        await _orderRepository.AddAsync(order, token);
        await _orderRepository.SaveChangesAsync(token);
    }
}