namespace Contracts.V1.Responses;

public sealed class TimeSeriesResponse
{
    public required string Interval { get; init; }
    public string? TimeZone { get; init; }
    public required ICollection<StockSnapshotResponse>? StockValues { get; init; }
}