namespace Application.Common.Models.ReadModels;

public sealed class TimeSeriesResponse
{
    public required string Status { get; set; }
    public required List<StockSnapshotRM> Values { get; set; }
    public required TimeSeriesMeta Meta { get; set; }
}