namespace Application.Common.Models.ReadModels;

public sealed class StockHiLoReadModel
{
    public required DateTime datetime { get; init; }
    public required decimal min { get; init; }
    public required decimal max { get; init; }
}