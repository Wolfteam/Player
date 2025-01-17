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
}