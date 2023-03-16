namespace Contracts.V1.Responses;

public sealed class StockResponse
{
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public required double Price { get; init; }
    public required string Currency { get; init; }
    public required string Country { get; init; }
    public required double Change { get; init; } 
    public double? TrendingScore { get; init; }
    public IList<TradeResponse>? Trades { get; init; }
    public TimeSeriesResponse? TimeSeries { get; init; }
}