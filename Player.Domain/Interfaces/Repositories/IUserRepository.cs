using Player.Domain.Entities;

namespace Player.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmail(string email);
    Task<User?> GetByEmail(string email);
}