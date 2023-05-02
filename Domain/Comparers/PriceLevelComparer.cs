public class PriceLevelComparerDescending : IComparer<IPriceLevel>
{
    public int Compare(IPriceLevel? x, IPriceLevel? y)
    {
        return y!.Price.CompareTo(x!.Price);
    }
    private static readonly PriceLevelComparerDescending _shared = new PriceLevelComparerDescending();
    public static PriceLevelComparerDescending Instance => _shared;
}
public class PriceLevelComparerAscending : IComparer<IPriceLevel>
{
    public int Compare(IPriceLevel? x, IPriceLevel? y)
    {
        return x!.Price.CompareTo(y!.Price);
    }
    private static readonly PriceLevelComparerAscending _shared = new PriceLevelComparerAscending();
    public static PriceLevelComparerAscending Instance => _shared;
}