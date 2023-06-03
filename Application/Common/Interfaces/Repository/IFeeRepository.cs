namespace Application.Common.Interfaces.Repository;

public interface IFeeRepository : IRepository<Fee>
{
     Task<bool> ExistAsync(int id, CancellationToken token);
     Task SaveChangesAsync(CancellationToken token);
}