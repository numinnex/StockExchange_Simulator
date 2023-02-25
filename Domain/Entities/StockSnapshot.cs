using Domain.Common;

namespace Domain.Entities;

public sealed class StockSnapshot : SubEntity
{
    public required double  Close { get; init; }
    public required double  Low { get; init; }
    public required double  High { get; init; }
    public required double  Open { get; init; }
    public required DateTime Datetime { get; init; }
    public required TimeSeries TimeSeries { get; init; }
}