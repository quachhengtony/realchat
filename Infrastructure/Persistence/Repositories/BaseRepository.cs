using Microsoft.EntityFrameworkCore;
using Realchat.Application.Repositories;
using Realchat.Domain.Common;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly DataContext _dataContext;

    public BaseRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public void Create(T entity)
    {
        _dataContext.Add(entity);
    }

    public void Delete(T entity)
    {
        _dataContext.Remove(entity);
    }

    public Task<List<T>> GetAll(CancellationToken cancellationToken)
    {
        return _dataContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await _dataContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Update(T entity)
    {
        _dataContext.Update(entity);
    }
}