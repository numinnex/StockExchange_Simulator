namespace Application.Common.Interfaces;

public interface IStockUtils
{
    Task<string> GetStockSymbolByStockId(Guid stockId);
    Task<bool> ExistsAsync(string stockId, CancellationToken token);
}