namespace Application.Common.Models.ReadModels;

public sealed class StockResponse
{
    public required List<StockReadModel> Data { get; set; }
    public required string Status { get; set; }
}