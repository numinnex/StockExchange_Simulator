using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Stock : Entity
{
    public required string Symbol { get; init; }
    public required string Name { get; init; }
    public required Price Price { get; set; }
    public required string Currency { get; init; }
    public required string Country { get; init; }
    public Change? Change { get; set; }
    public double? TrendingScore { get; set; }
    public IEnumerable<MarketOrder>? MarketOrders { get; set; }
    public int? TimeSeriesId { get; set; }
    public TimeSeries? TimeSeries { get; set; }
}