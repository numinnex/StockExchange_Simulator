namespace Application.Common.Models.ReadModels;

public sealed class TimeSeriesMeta
{
    public required string Symbol { get; set; }
    public required string Interval { get; set; }
    public string? exchange_timezone { get; set; }
}