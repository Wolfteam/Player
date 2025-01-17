using Player.Domain.Entities;

namespace Player.Domain.Interfaces.Repositories;

public interface IMediaRepository : IBaseRepository<Media>
{
    Task CreateMedia(Media media);
    Task DeleteMedia(long id);
}