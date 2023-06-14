
using Domain.PriceLevels;
using Domain.ValueObjects;

public interface IBook
{
    public IEnumerable<QuantityTrackingPriceLevel> BidSide { get; }
    public IEnumerable<QuantityTrackingPriceLevel> AskSide { get; }
    public IEnumerable<PriceLevel> StopBidSide { get; }
    public IEnumerable<PriceLevel> StopAskSide { get; }
    public Price? CurrentMarketPrice { get; } 
    public Price? BestStopBidPrice { get; }
    public Price? BestStopAskPrice { get; }
    public int BidLevelsCount { get; }
    public int AskLevelsCount { get; }
    public int BestBidsCount { get; }
    public int BestAsksCount { get; }
    public int StopBidsLevelCount { get; }
    public int StopAsksLevelCount { get; }
    public int BestStopBidsCount { get; }
    public int BestStopAsksCount { get; }
    public IReadOnlyList<PriceLevel> RemoveStopBidsUntillPrice(Price price);
    public IReadOnlyList<PriceLevel> RemoveStopAsksUntillPrice(Price price);
    public bool RemoveOrder(IOrder order, Price price);
    public bool AddOrder(IOrder order );
    public bool FillOrder(IOrder order, Quantity quantity, Price price);
    public bool CheckCanFillMarketOrderAmount(bool isBuy, Amount orderAmount);
    public IOrder? GetBestOffer(bool isBuy, string userId);
    public void ClearBook();

}