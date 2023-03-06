using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Stock : Entity
{
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public required Price Price { get; set; }
    public required string Currency { get; set; }
    public required string Country { get; set; }
    public required Change Change { get; set; } 
    public double? TrendingScore { get; set; }
    public IEnumerable<Trade>? Trades { get; set; }
    public IEnumerable<TimeSeries>? TimeSeries { get; set; }
}