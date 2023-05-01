public class Book : IBook
{
    private readonly Side<QuantityTrackingPriceLevel> _bids;
    private readonly Side<QuantityTrackingPriceLevel> _asks;

    public IEnumerable<QuantityTrackingPriceLevel> BidSide => throw new NotImplementedException();

    public IEnumerable<QuantityTrackingPriceLevel> AskSide => throw new NotImplementedException();
}