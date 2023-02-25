using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Stocks.Queries;

public sealed class StockDto
{
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public required Price Price { get; init; }
    public required string Currency { get; init; }
    public required string Country { get; init; }
    public required Change Change { get; init; } 
    public double? TrendingScore { get; init; }
    public IList<Trade>? Trades { get; init; }
    public IList<TimeSeries>? TimeSeries { get; init; }
}