using System.Security;
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
    private readonly IPortfolioRepository _portfolioRepository;
    public TradeListener(ITradeRepository tradeRepository, IOrderRepository orderRepository,
        IDateTimeProvider dateTimeProvider, ILogger<TradeListener> logger, IPortfolioRepository portfolioRepository)
    {
        _tradeRepository = tradeRepository;
        _orderRepository = orderRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _portfolioRepository = portfolioRepository;
    }
    public async Task OnTradeAsync(TradeFootprint tradeFootprint, CancellationToken token)
    {
        await _tradeRepository.AddTradeFootprintAsync(tradeFootprint, token);
        await _tradeRepository.SaveChangesAsync(token);
        //this is bad, this whole logic should be encapsulated in some sort of service
        if (tradeFootprint.ProcessedOrderIsBuy)
        {
            var buyerPortfolio = await _portfolioRepository.GetByUserIdAsync(tradeFootprint.ProcessedOrderUserId, token);
            var sellerPortfolio = await _portfolioRepository.GetByUserIdAsync(tradeFootprint.RestingOrderUserId, token);
            if (buyerPortfolio is not null && sellerPortfolio is not null)
            {
                var buyerSecurity = buyerPortfolio.Securities
                    .FirstOrDefault(x => x.OrderId == tradeFootprint.ProcessedOrderId);
                if (buyerSecurity is null)
                {
                    buyerSecurity = new Security
                    {
                        Quantity = tradeFootprint.Quantity,
                        OrderId = tradeFootprint.ProcessedOrderId,
                        Timestamp = _dateTimeProvider.Now,
                        PortfolioId = buyerPortfolio.Id,
                        PurchasedPrice = tradeFootprint.MatchPrice,
                        StockId = tradeFootprint.StockId,
                    };
                    await _portfolioRepository.AddSecurityAsync(buyerSecurity , token);
                    await _portfolioRepository.SaveChangesAsync(token);
                }
                else
                {
                    buyerSecurity!.Quantity += tradeFootprint.Quantity;
                    _portfolioRepository.UpdateSecurity(buyerSecurity);
                    await _portfolioRepository.SaveChangesAsync(token);
                }
                var sellerSecurity = sellerPortfolio.Securities
                    .FirstOrDefault(x => x.OrderId == tradeFootprint.RestingOrderId);
                sellerSecurity!.Quantity -= tradeFootprint.Quantity;
                _portfolioRepository.UpdateSecurity(sellerSecurity);
                await _portfolioRepository.SaveChangesAsync(token);
            }
        }
        else
        {
            var sellerPortfolio = await _portfolioRepository.GetByUserIdAsync(tradeFootprint.ProcessedOrderUserId, token);
            var buyerPortfolio =
                await _portfolioRepository.GetByUserIdAsync(tradeFootprint.RestingOrderUserId, token);
            if (buyerPortfolio is not null && sellerPortfolio is not null)
            {
                var sellerSecurity = sellerPortfolio.Securities
                    .FirstOrDefault(x => x.OrderId == tradeFootprint.ProcessedOrderId);
                var buyerSecurity = buyerPortfolio.Securities
                    .FirstOrDefault(x => x.OrderId == tradeFootprint.RestingOrderId);
                if (buyerSecurity is null)
                {
                    buyerSecurity = new Security
                    {
                        Quantity = tradeFootprint.Quantity,
                        OrderId = tradeFootprint.RestingOrderId,
                        Timestamp = _dateTimeProvider.Now,
                        PortfolioId = buyerPortfolio.Id,
                        PurchasedPrice = tradeFootprint.MatchPrice,
                        StockId = tradeFootprint.StockId,
                    };
                    await _portfolioRepository.AddSecurityAsync(buyerSecurity, token);
                    await _portfolioRepository.SaveChangesAsync(token);
                }
                else
                {
                    buyerSecurity!.Quantity += tradeFootprint.Quantity;
                    _portfolioRepository.UpdateSecurity(buyerSecurity);
                    await _portfolioRepository.SaveChangesAsync(token);
                }

                _portfolioRepository.UpdateSecurity(buyerSecurity);
                sellerSecurity!.Quantity -= tradeFootprint.Quantity;
                _portfolioRepository.UpdateSecurity(sellerSecurity);
                await _portfolioRepository.SaveChangesAsync(token);

            }
        }
        
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