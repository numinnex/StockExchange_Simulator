using Domain.Common;

namespace Domain.Entities;

public sealed class TimeSeries : SubEntity
{
    public required Stock Stock { get; init; }
    public required string Inteval { get; init; }
    public string? TimeZone { get; init; }
    public required IEnumerable<StockSnapshot> StockValues { get; init; }
}


