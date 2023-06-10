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
            var boughtSecurity = new Security
            {
                Quantity = tradeFootprint.Quantity,
                StockId = tradeFootprint.ProcessedOrderId,
                UserId = tradeFootprint.ProcessedOrderUserId,
                PurchasedPrice = tradeFootprint.MatchPrice,
            };
            var buyerPortfolio =
                await _portfolioRepository.GetByUserIdAsync(tradeFootprint.ProcessedOrderUserId, token);
            var sellerPortfolio = await _portfolioRepository.GetByUserIdAsync(tradeFootprint.RestingOrderUserId, token);

            await _portfolioRepository.AddSecurityAsync(boughtSecurity, token);
            await _portfolioRepository.SaveChangesAsync(token);

            buyerPortfolio!.TotalValue += tradeFootprint.TradeDetails.BidCost;
            sellerPortfolio!.TotalValue -= tradeFootprint.TradeDetails.BidCost;
            _portfolioRepository.UpdatePortfolio(buyerPortfolio);
            _portfolioRepository.UpdatePortfolio(sellerPortfolio);
            await _portfolioRepository.SaveChangesAsync(token);

            var sellerSecurity = await _portfolioRepository.GetSecurityByUserIdAndStockId(
                tradeFootprint.RestingOrderId.ToString(), tradeFootprint.RestingOrderUserId, token);

            sellerSecurity!.Quantity -= tradeFootprint.Quantity;
            if (sellerSecurity.Quantity == 0)
            {
                _portfolioRepository.RemoveSecurity(sellerSecurity);
                await _portfolioRepository.SaveChangesAsync(token);
            }
            else
            {
                _portfolioRepository.UpdateSecurity(sellerSecurity);
                await _portfolioRepository.SaveChangesAsync(token);
            }
        }
        else
        {
            var sellerSecurity = await _portfolioRepository.GetSecurityByUserIdAndStockId(
                tradeFootprint.RestingOrderId.ToString(), tradeFootprint.RestingOrderUserId, token);
            var buyerPortfolio =
                await _portfolioRepository.GetByUserIdAsync(tradeFootprint.ProcessedOrderUserId, token);
            var sellerPortfolio = await _portfolioRepository.GetByUserIdAsync(tradeFootprint.RestingOrderUserId, token);
            sellerSecurity!.Quantity -= tradeFootprint.Quantity;
            if (sellerSecurity.Quantity <= 0)
            {
                _portfolioRepository.RemoveSecurity(sellerSecurity);
            }
            else
            {
                _portfolioRepository.UpdateSecurity(sellerSecurity);
            }

            buyerPortfolio!.TotalValue += tradeFootprint.TradeDetails.BidCost;
            sellerPortfolio!.TotalValue -= tradeFootprint.TradeDetails.BidCost;
            _portfolioRepository.UpdatePortfolio(buyerPortfolio);
            _portfolioRepository.UpdatePortfolio(sellerPortfolio);
            await _portfolioRepository.SaveChangesAsync(token);

            var boughtSecurity = new Security
            {
                Quantity = tradeFootprint.Quantity,
                StockId = tradeFootprint.ProcessedOrderId,
                UserId = tradeFootprint.ProcessedOrderUserId,
            };
            await _portfolioRepository.AddSecurityAsync(boughtSecurity, token);
            await _portfolioRepository.SaveChangesAsync(token);
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