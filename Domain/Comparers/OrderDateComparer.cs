public class OrderDateComparer : IComparer<IOrder>
{
    public int Compare(IOrder? x, IOrder? y)
    {
        return x!.Timestamp.CompareTo(y!.Timestamp);
    }
    private static readonly OrderDateComparer _shared = new OrderDateComparer();
    public static OrderDateComparer Instance => _shared;
}