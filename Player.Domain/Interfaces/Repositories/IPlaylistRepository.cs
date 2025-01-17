using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Entities;

namespace Player.Domain.Interfaces.Repositories;

public interface IPlaylistRepository : IBaseRepository<Playlist>
{
    Task<bool> ExistsByIdAndUserId(long id, long userId);
    Task<List<PlaylistResponseDto>> GetAllByUserId(long userId);
}