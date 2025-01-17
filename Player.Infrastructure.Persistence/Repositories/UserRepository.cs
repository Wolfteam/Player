using Player.Domain.Entities;
using Player.Domain.Interfaces.Repositories;
using Player.Domain.Utils;

namespace Player.Infrastructure.Persistence.Repositories;

internal class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IFreeSql context)
        : base(context)
    {
    }

    public Task<bool> ExistsByEmail(string email)
    {
        Check.NotEmpty(email, nameof(email));
        return Context
            .Select<User>()
            .Where(u => u.Email == email)
            .AnyAsync();
    }

    public async Task<User?> GetByEmail(string email)
    {
        Check.NotEmpty(email, nameof(email));
        List<User> results = await Context
            .Select<User>()
            .Where(u => u.Email == email)
            .ToListAsync();

        return results.FirstOrDefault();
    }
}