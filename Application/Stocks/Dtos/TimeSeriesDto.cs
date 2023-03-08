namespace Application.Stocks.Dtos;

public sealed class TimeSeriesDto
{
    public required string Interval { get; init; }
    public string? TimeZone { get; init; }
    public required ICollection<StockSnapShotDto>? StockValues { get; init; }
}
