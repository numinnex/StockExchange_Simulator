namespace Contracts.V1.Responses;

public sealed class StockSnapshotResponse
{
    public required double  Close { get; init; }
    public required double  Low { get; init; }
    public required double  High { get; init; }
    public required double  Open { get; init; }
    public required DateTime Datetime { get; init; }
}