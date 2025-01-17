using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Playlists;
using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Interfaces;

namespace Player.Application.Playlists;

public interface IPlaylistService
{
    Task<ListResultDto<PlaylistResponseDto>> GetAllPlaylists(ICurrentLoggedUser currentLoggedUser);

    Task<ResultDto<PlaylistResponseDto>> CreatePlaylist(
        CreatePlaylistRequestDto dto,
        ICurrentLoggedUser currentLoggedUser);

    Task<ResultDto<PlaylistResponseDto>> UpdatePlaylist(
        long id,
        UpdatePlaylistRequestDto dto,
        ICurrentLoggedUser currentLoggedUser);

    Task<EmptyResultDto> DeletePlaylist(long id, ICurrentLoggedUser currentLoggedUser);
}