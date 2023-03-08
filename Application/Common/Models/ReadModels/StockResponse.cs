namespace Application.Common.Models.ReadModels;

public sealed class StockResponse
{
    public required IEnumerable<StockReadModel> Data { get; set; }
    public required string Status { get; set; }
}