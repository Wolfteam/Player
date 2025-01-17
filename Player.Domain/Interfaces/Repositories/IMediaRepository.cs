using Player.Domain.Entities;

namespace Player.Domain.Interfaces.Repositories;

public interface IMediaRepository : IBaseRepository<Media>
{
    Task<bool> ExistsByIdAndPlayListId(long id, long playListId);

    Task<List<Media>> GetAllByPlayListId(long playListId);

    Task<long> GetMediaCountByPlayListId(long playListId);
}