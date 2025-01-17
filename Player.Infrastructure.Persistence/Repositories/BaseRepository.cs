using Player.Domain.Interfaces;
using Player.Domain.Interfaces.Repositories;
using Player.Domain.Utils;

namespace Player.Infrastructure.Persistence.Repositories;

internal class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IBaseEntity
{
    protected readonly IFreeSql Context;

    public BaseRepository(IFreeSql context)
    {
        Context = context;
    }

    public virtual async Task<TEntity?> GetById(long id)
    {
        Check.NotEmpty(id, nameof(id));

        List<TEntity> entities = await Context
            .Select<TEntity>()
            .Where(u => u.Id == id)
            .ToListAsync();

        return entities.FirstOrDefault();
    }

    public virtual Task<bool> ExistsById(long id)
    {
        Check.NotEmpty(id, nameof(id));
        return Context
            .Select<TEntity>()
            .Where(u => u.Id == id)
            .AnyAsync();
    }

    public virtual async Task Create(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.Id = await Context.Insert(entity).ExecuteIdentityAsync();
    }

    public virtual Task Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        if (entity.Id == 0)
        {
            return Task.CompletedTask;
        }
        return Context.Update<TEntity>()
            .SetSource(entity)
            .ExecuteAffrowsAsync();
    }

    public virtual Task Delete(long id)
    {
        Check.NotEmpty(id, nameof(id));
        return Context.Delete<TEntity>().Where(u => u.Id == id).ExecuteAffrowsAsync();
    }
}