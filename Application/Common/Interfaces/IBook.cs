
using Application.Common.Models;
using Domain.ValueObjects;

public interface IBook
{
    public IEnumerable<QuantityTrackingPriceLevel> BidSide { get; }
    public IEnumerable<QuantityTrackingPriceLevel> AskSide { get; }
    public int BidLevelsCount { get; }
    public int AskLevelsCount { get; }
    public bool RemoveOrder(IOrder order, Price price);
    public bool AddOrder(IOrder order, Price price);
    public bool FillOrder(IOrder order, Quantity quantity, Price price);
    public bool CheckCanFillMarketOrderAmount(bool isBuy, Amount orderAmount);
    public IOrder? GetBestOffer(bool isBuy, string userId);

}