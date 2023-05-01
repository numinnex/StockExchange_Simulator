public class PriceLevelComparer : IComparer<IPriceLevel>
{
    public int Compare(IPriceLevel? x, IPriceLevel? y)
    {
        return x!.Price.CompareTo(y!.Price);
    }
    private static readonly PriceLevelComparer _shared = new PriceLevelComparer();
    public static PriceLevelComparer Instance => _shared;
}