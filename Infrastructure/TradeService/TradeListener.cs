using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.TradeService;

public sealed class TradeListener : ITradeListener
{
    private readonly ITradeRepository _tradeRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<TradeListener> _logger;

    public TradeListener(ITradeRepository tradeRepository, IOrderRepository orderRepository,
        IDateTimeProvider dateTimeProvider, ILogger<TradeListener> logger)
    {
        _tradeRepository = tradeRepository;
        _orderRepository = orderRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }
    public async Task OnTradeAsync(TradeFootprint tradeFootprint, CancellationToken token)
    {
        await _tradeRepository.AddTradeFootprintAsync(tradeFootprint, token);
        await _tradeRepository.SaveChangesAsync(token);
        _logger.LogInformation("A trade has been made at {DateTime}, between {processedUserId} and {restingUserId}.Processed Order ID:{processedOrder}, Resting Order ID:{restingOrder}.Traded {quantity} quantity for {price}.",
            _dateTimeProvider.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            tradeFootprint.ProcessedOrderUserId,
            tradeFootprint.RestingOrderUserId,
            tradeFootprint.ProcessedOrderId,
            tradeFootprint.RestingOrderId,
            tradeFootprint.Quantity,
            tradeFootprint.MatchPrice);
    }

    public async Task OnAcceptAsync(IOrder order, CancellationToken token)
    {
        try
        {
            await _orderRepository.AddAsync(order, token);
            _logger.LogInformation("Succesfully added order to database made by {userId} for {symbol} at {datetime}",
                order.UserId,
                order.Symbol,
                _dateTimeProvider.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        catch
        {
            _logger.LogError("Failed to add order to database made by {userId} for {symbol} at {datetime}",
                order.UserId,
                order.Symbol,
                _dateTimeProvider.Now.ToString("yyyy-MM-dd HH:mm:ss")
            ); 
        }
    }
}