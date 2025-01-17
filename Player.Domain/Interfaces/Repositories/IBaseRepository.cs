namespace Player.Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity> where TEntity : IBaseEntity
{
    Task<TEntity?> GetById(long id);

    Task<bool> ExistsById(long id);

    Task Create(TEntity entity);

    Task Update(TEntity entity);

    Task Delete(long id);
}