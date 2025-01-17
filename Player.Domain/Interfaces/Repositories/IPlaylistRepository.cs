using Player.Domain.Entities;

namespace Player.Domain.Interfaces.Repositories;

public interface IPlaylistRepository : IBaseRepository<Playlist>
{
    Task CreatePlaylist(Playlist playlist);
    Task UpdatePlaylistName(long id, string name);
    Task<bool> ExistsByName(string name);
    Task DeletePlaylist(long id);
}