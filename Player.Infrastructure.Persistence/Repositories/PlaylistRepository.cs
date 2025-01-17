using Player.Domain.Dtos.Responses.Playlists;
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

    public override async Task Delete(long id)
    {
        Check.NotEmpty(id, nameof(id));
        using (var uow = Context.CreateUnitOfWork())
        {
            await Context.Delete<Media>().Where(m => m.PlaylistId == id).ExecuteAffrowsAsync();
            await base.Delete(id);
            uow.Commit();
        }
    }

    public Task<bool> ExistsByIdAndUserId(long id, long userId)
    {
        Check.NotEmpty(id, nameof(id));
        Check.NotEmpty(userId, nameof(userId));
        return Context
            .Select<Playlist>()
            .Where(u => u.Id == id && u.UserId == userId)
            .AnyAsync();
    }

    public Task<List<PlaylistResponseDto>> GetAllByUserId(long userId)
    {
        Check.NotEmpty(userId, nameof(userId));
        return Context
            .Select<Playlist>()
            .Where(u => u.UserId == userId)
            .ToListAsync(pl => new PlaylistResponseDto(
                pl.Id,
                pl.Name,
                Context.Select<Media>().Where(m => m.PlaylistId == pl.Id).Count())
            );
    }
}