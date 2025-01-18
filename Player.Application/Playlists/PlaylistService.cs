using Microsoft.Extensions.Logging;
using Player.Application.Validation;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Playlists;
using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Entities;
using Player.Domain.Interfaces;
using Player.Domain.Interfaces.Services;
using Player.Domain.Utils;

namespace Player.Application.Playlists;

internal class PlaylistService : IPlaylistService
{
    private readonly ILogger _logger;
    private readonly IValidatorService _validator;
    private readonly IAppDataService _dataService;

    public PlaylistService(ILoggerFactory loggerFactory, IValidatorService validator, IAppDataService dataService)
    {
        _logger = loggerFactory.CreateLogger(GetType());
        _validator = validator;
        _dataService = dataService;
    }

    public async Task<ListResultDto<PlaylistResponseDto>> GetAllPlaylists(ICurrentLoggedUser currentLoggedUser)
    {
        _logger.LogInformation("Retrieving all playlists that belongs to userId = {UserId}", currentLoggedUser.Id);
        List<PlaylistResponseDto> playlists = await _dataService.Playlists.GetAllByUserId(currentLoggedUser.Id);

        _logger.LogInformation(
            "Got {Count} playlists that belongs to userId = {UserId}",
            playlists.Count,
            currentLoggedUser.Id);
        return ListResult.Success(playlists);
    }

    public async Task<ResultDto<PlaylistResponseDto>> CreatePlaylist(
        CreatePlaylistRequestDto dto,
        ICurrentLoggedUser currentLoggedUser)
    {
        EmptyResultDto validationResult = _validator.Validate(dto);
        if (!validationResult.Succeed)
        {
            return Result.FromOther<PlaylistResponseDto>(validationResult);
        }

        _logger.LogInformation(
            "Creating playlist = {Name} for userId = {UserId}",
            dto.Name,
            currentLoggedUser.Id);
        var playList = new Playlist
        {
            UserId = currentLoggedUser.Id,
            Name = dto.Name
        };

        await _dataService.Playlists.Create(playList);

        _logger.LogInformation(
            "PlaylistId = {Id} was successfully created for userId = {UserId}",
            playList.Id,
            currentLoggedUser.Id);
        var result = new PlaylistResponseDto(playList.Id, playList.Name, 0);

        return Result.Success(result);
    }

    public async Task<ResultDto<PlaylistResponseDto>> UpdatePlaylist(
        long id,
        UpdatePlaylistRequestDto dto,
        ICurrentLoggedUser currentLoggedUser)
    {
        if (Check.IsEmpty(id))
        {
            return Result.InvalidId<PlaylistResponseDto>(id);
        }

        EmptyResultDto validationResult = _validator.Validate(dto);
        if (!validationResult.Succeed)
        {
            return Result.FromOther<PlaylistResponseDto>(validationResult);
        }

        Playlist? playList = await _dataService.Playlists.GetById(id);
        if (playList == null || playList.UserId != currentLoggedUser.Id)
        {
            return Result.NotFound<PlaylistResponseDto>($"PlaylistId = {id} does not exist");
        }

        _logger.LogInformation("Updating playlistId = {Id} for userId = {UserId}", id, currentLoggedUser.Id);
        playList.Name = dto.Name;
        await _dataService.Playlists.Update(playList);

        _logger.LogInformation(
            "PlaylistId = {Id} was successfully updated for userId = {UserId}",
            playList.Id,
            currentLoggedUser.Id);
        long mediaCount = await _dataService.Medias.GetMediaCountByPlayListId(id);
        var result = new PlaylistResponseDto(playList.Id, playList.Name, mediaCount);

        return Result.Success(result);
    }

    public async Task<EmptyResultDto> DeletePlaylist(long id, ICurrentLoggedUser currentLoggedUser)
    {
        if (Check.IsEmpty(id))
        {
            return EmptyResult.InvalidId(id);
        }

        if (!await _dataService.Playlists.ExistsByIdAndUserId(id, currentLoggedUser.Id))
        {
            return EmptyResult.NotFound($"PlaylistId = {id} does not exist");
        }

        _logger.LogInformation("Deleting playListId = {Id}...", id);
        await _dataService.Playlists.Delete(id);

        _logger.LogInformation("PlayListId = {Id} was successfully deleted", id);
        return EmptyResult.Success();
    }
}