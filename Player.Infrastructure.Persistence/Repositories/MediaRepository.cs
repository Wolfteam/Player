using Player.Domain.Entities;
using Player.Domain.Interfaces.Repositories;
using Player.Domain.Utils;

namespace Player.Infrastructure.Persistence.Repositories;

internal class MediaRepository : BaseRepository<Media>, IMediaRepository
{
    public MediaRepository(IFreeSql context)
        : base(context)
    {
    }

    public Task<bool> ExistsByIdAndPlayListId(long id, long playListId)
    {
        Check.NotEmpty(id, nameof(id));
        Check.NotEmpty(playListId, nameof(playListId));

        return Context.Select<Media>()
            .Where(m => m.Id == id && m.PlaylistId == playListId)
            .AnyAsync();
    }

    public Task<List<Media>> GetAllByPlayListId(long playListId)
    {
        Check.NotEmpty(playListId, nameof(playListId));

        return Context.Select<Media>()
            .Where(m => m.PlaylistId == playListId)
            .ToListAsync();
    }

    public Task<long> GetMediaCountByPlayListId(long playListId)
    {
        Check.NotEmpty(playListId, nameof(playListId));
        return Context.Select<Media>()
            .Where(m => m.PlaylistId == playListId)
            .CountAsync();
    }
}