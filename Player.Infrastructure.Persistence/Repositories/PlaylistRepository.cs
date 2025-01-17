using Player.Domain.Entities;
using Player.Domain.Interfaces.Repositories;
using Player.Domain.Utils;

namespace Player.Infrastructure.Persistence.Repositories;

internal class PlaylistRepository : BaseRepository<Playlist>, IPlaylistRepository
{
    public PlaylistRepository(IFreeSql context)
        : base(context)
    {
    }

    public Task<bool> ExistsByName(string name)
    {
        Check.NotEmpty(name, nameof(name));
        return Context
            .Select<Playlist>()
            .Where(u => u.Name == name)
            .AnyAsync();
    }

    public override async Task Delete(long id)
    {
        Check.NotEmpty(id, nameof(id));
        using (var uow = Context.CreateUnitOfWork())
        {
            await Context.Delete<Playlist>().Where(pl => pl.Id == id).ExecuteDeletedAsync();
            await Context.Delete<Media>().Where(m => m.PlaylistId == id).ExecuteDeletedAsync();
            uow.Commit();
        }
    }
}