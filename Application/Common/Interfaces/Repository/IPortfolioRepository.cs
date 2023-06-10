using Domain.Entities;

namespace Application.Common.Interfaces.Repository;

//this interface is maybe too big, split it into smaller ones (e.g ISecurityRepository, IPortfolioRepository)
public interface IPortfolioRepository: IRepository<Portfolio>
{
    void UpdatePortfolio(Portfolio portfolio);
    Task<List<Portfolio>> GetAllAsync(CancellationToken token);
    Task<Portfolio?> GetByUserIdAsync(string userId, CancellationToken token);
    void RemoveSecurity(Security security);
    Task<List<Security>> GetSecuritiesByUserIdAsync(string userId, CancellationToken token);
    Task<Security?> GetSecurityByUserIdAndStockId(string stockId,string userId, CancellationToken token);
    Task AddSecurityAsync(Security security, CancellationToken token);
    Task AddValueSnapshots(List<ValueSnapshot> values, CancellationToken token);
    void UpdateSecurity(Security security );
    Task SaveChangesAsync(CancellationToken token);
}