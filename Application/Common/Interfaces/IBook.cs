
public interface IBook
{
    public IEnumerable<QuantityTrackingPriceLevel> BidSide { get; }
    public IEnumerable<QuantityTrackingPriceLevel> AskSide { get; }
}