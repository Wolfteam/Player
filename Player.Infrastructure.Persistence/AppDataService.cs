using Player.Domain.Interfaces.Repositories;
using Player.Domain.Interfaces.Services;
using Player.Infrastructure.Persistence.Repositories;

namespace Player.Infrastructure.Persistence;

internal class AppDataService : IAppDataService
{
    public IUserRepository Users { get; }

    public IPlaylistRepository Playlists { get; }

    public IMediaRepository Medias { get; }

    public AppDataService(IFreeSql context)
    {
        Users = new UserRepository(context);
        Playlists = new PlaylistRepository(context);
        Medias = new MediaRepository(context);
    }
}