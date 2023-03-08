using Domain.Common;

namespace Domain.Entities;

public sealed class TimeSeries : SubEntity
{
    public Stock? Stock { get; init; }
    public required string Interval { get; init; }
    public string? TimeZone { get; init; }
    public required ICollection<StockSnapshot>? StockValues { get; init; }
}


