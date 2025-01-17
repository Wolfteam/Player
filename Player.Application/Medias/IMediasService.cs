using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Medias;
using Player.Domain.Dtos.Responses.Medias;
using Player.Domain.Interfaces;

namespace Player.Application.Medias;

public interface IMediasService
{
    Task<ListResultDto<GetAllMediaResponseDto>> GetAllMedias(
        long playListId,
        ICurrentLoggedUser currentLoggedUser);

    Task<ResultDto<GetAllMediaResponseDto>> CreateMedia(
        long playListId,
        CreateMediaRequestDto dto,
        ICurrentLoggedUser currentLoggedUser);

    Task<EmptyResultDto> DeleteMedia(long playListId, long id, ICurrentLoggedUser currentLoggedUser);
    Task<ResultDto<byte[]>> GetMediaData(long playListId, long id, ICurrentLoggedUser currentLoggedUser);
}