using Domain.ValueObjects;

public class PriceComparerDescending : IComparer<Price>
{
    public int Compare(Price? x, Price? y)
    {
        return y!.CompareTo(x!);
    }
    private static readonly PriceComparerDescending _shared = new();
    public static PriceComparerDescending Instance => _shared;
}
public class PriceComparerAscending : IComparer<Price>
{
    public int Compare(Price? x, Price? y)
    {
        return x!.CompareTo(y!);
    }
    private static readonly PriceComparerAscending _shared = new();
    public static PriceComparerAscending Instance => _shared;
}