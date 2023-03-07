using System.Linq.Expressions;
using Application.Common.Interfaces.Repository;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _ctx;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
        _dbSet = _ctx.Set<T>();
    }
    
    public async Task<List<T>> GetAllAsync(CancellationToken token,Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;

        if (includeProperties is not null)
        {
            foreach (var prop in includeProperties.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query.Include(prop);
            }
        }

        if (filter is not null)
            query = query.Where(filter);

        return await query.ToListAsync(token);
    }


    public async Task AddAsync(T entity, CancellationToken token)
    {
        await _dbSet.AddAsync(entity, token);
    }

    public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken token,string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;

        if (includeProperties is not null)
        {
            foreach (var prop in includeProperties.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query.Include(prop);
            }
        }
        
        return await query.FirstOrDefaultAsync(filter, token);
    }

    public void Remove(T entity )
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities )
    {
        _dbSet.RemoveRange(entities);
    }
}