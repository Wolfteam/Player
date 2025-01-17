using Player.Domain.Interfaces.Repositories;

namespace Player.Domain.Interfaces.Services;

public interface IAppDataService
{
    IUserRepository Users { get; }
    IPlaylistRepository Playlists { get; }
    IMediaRepository Medias { get; }
}