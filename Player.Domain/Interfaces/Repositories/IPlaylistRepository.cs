using Player.Domain.Entities;

namespace Player.Domain.Interfaces.Repositories;

public interface IPlaylistRepository : IBaseRepository<Playlist>
{
    Task<bool> ExistsByName(string name);
}