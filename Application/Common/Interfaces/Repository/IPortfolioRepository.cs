using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

public interface IPortfolioRepository: IRepository<Portfolio>
{
    Task<List<Portfolio>> GetAllAsync(CancellationToken token);
    void RemoveSecurity(Security security);
    Task<List<Security>> GetAllUserSecuritiesAsync(string userId, CancellationToken token);
    Task<List<Security>> GetSecuritiesByUserIdAsync(string userId, CancellationToken token);
    Task<Security?> GetSecurityByUserIdAndStockId(string stockId,string userId, CancellationToken token);
    Task AddSecurityAsync(Security security, CancellationToken token);
    void UpdateSecurity(Security security );
    Task SaveChangesAsync(CancellationToken token);
}