using Microsoft.Extensions.Logging;
using Player.Application.Validation;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Medias;
using Player.Domain.Dtos.Responses.Medias;
using Player.Domain.Entities;
using Player.Domain.Interfaces;
using Player.Domain.Interfaces.Services;
using Player.Domain.Utils;

namespace Player.Application.Medias;

internal class MediasService : IMediasService
{
    private readonly ILogger _logger;
    private readonly IValidatorService _validator;
    private readonly IAppDataService _dataService;

    public MediasService(ILoggerFactory loggerFactory, IValidatorService validator, IAppDataService dataService)
    {
        _logger = loggerFactory.CreateLogger(GetType());
        _validator = validator;
        _dataService = dataService;
    }

    public async Task<ListResultDto<GetAllMediaResponseDto>> GetAllMedias(
        long playListId,
        ICurrentLoggedUser currentLoggedUser)
    {
        if (Check.IsEmpty(playListId))
        {
            return ListResult.InvalidId<GetAllMediaResponseDto>(playListId);
        }

        if (!await _dataService.Playlists.ExistsByIdAndUserId(playListId, currentLoggedUser.Id))
        {
            _logger.LogWarning(
                "PlaylistId = {Id} associated to userId = {UserId} does not exist",
                playListId,
                currentLoggedUser.Id);
            return ListResult.NotFound<GetAllMediaResponseDto>($"PlaylistId = {playListId} does not exist");
        }

        _logger.LogInformation("Getting all medias associated to playlistId = {Id}...", playListId);
        List<Media> medias = await _dataService.Medias.GetAllByPlayListId(playListId);
        var result = medias
            .ConvertAll(m => new GetAllMediaResponseDto(m.Id, m.PlaylistId, m.Name, m.Length, m.LengthInSeconds));

        _logger.LogInformation("Got {Count} medias associated to playlistId = {Id}", medias.Count, playListId);
        return ListResult.Success(result);
    }

    public async Task<ResultDto<GetAllMediaResponseDto>> CreateMedia(
        long playListId,
        CreateMediaRequestDto dto,
        ICurrentLoggedUser currentLoggedUser)
    {
        if (Check.IsEmpty(playListId))
        {
            return Result.InvalidId<GetAllMediaResponseDto>(playListId);
        }

        EmptyResultDto validationResult = _validator.Validate(dto);
        if (!validationResult.Succeed)
        {
            return Result.FromOther<GetAllMediaResponseDto>(validationResult);
        }

        if (!await _dataService.Playlists.ExistsByIdAndUserId(playListId, currentLoggedUser.Id))
        {
            _logger.LogWarning(
                "PlaylistId = {Id} associated to userId = {UserId} does not exist",
                playListId,
                currentLoggedUser.Id);
            return Result.NotFound<GetAllMediaResponseDto>($"PlaylistId = {playListId} does not exist");
        }

        try
        {
            _logger.LogInformation("Creating media = {Name} on playListId = {Id}...", dto.Name, playListId);
            string path = await SaveMediaData(dto);

            var format = WavFormat.FromStream(dto.Data);
            var media = new Media
            {
                Name = dto.Name,
                Length = format.Length,
                LengthInSeconds = format.LengthInSeconds,
                PlaylistId = playListId,
                Path = path
            };

            await _dataService.Medias.Create(media);

            _logger.LogInformation("MediaId = {Id} was successfully created", media.Id);
            var result = new GetAllMediaResponseDto(
                media.Id,
                media.PlaylistId,
                media.Name,
                media.Length,
                media.LengthInSeconds);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unknown error occurred while saving media data");
            throw;
        }
    }

    public async Task<EmptyResultDto> DeleteMedia(long playListId, long id, ICurrentLoggedUser currentLoggedUser)
    {
        if (Check.IsEmpty(playListId))
        {
            return EmptyResult.InvalidId(playListId);
        }

        if (Check.IsEmpty(id))
        {
            return EmptyResult.InvalidId(id);
        }

        if (!await _dataService.Playlists.ExistsByIdAndUserId(playListId, currentLoggedUser.Id))
        {
            _logger.LogWarning(
                "PlaylistId = {Id} associated to userId = {UserId} does not exist",
                playListId,
                currentLoggedUser.Id);
            return EmptyResult.NotFound($"PlaylistId = {playListId} does not exist");
        }

        Media? media = await _dataService.Medias.GetById(id);
        if (media == null || media.PlaylistId != playListId)
        {
            _logger.LogWarning("MediaId = {Id} does not exist", id);
            return EmptyResult.NotFound($"MediaId = {id} does not exist");
        }

        _logger.LogInformation("Deleting mediaId = {Id}...", id);
        if (File.Exists(media.Path))
        {
            File.Delete(media.Path);
        }

        await _dataService.Medias.Delete(id);

        _logger.LogInformation("MediaId = {Id} was successfully deleted", id);
        return EmptyResult.Success();
    }

    public async Task<ResultDto<byte[]>> GetMediaData(long playListId, long id, ICurrentLoggedUser currentLoggedUser)
    {
        if (Check.IsEmpty(playListId))
        {
            return Result.InvalidId<byte[]>(playListId);
        }

        if (Check.IsEmpty(id))
        {
            return Result.InvalidId<byte[]>(id);
        }

        if (!await _dataService.Playlists.ExistsByIdAndUserId(playListId, currentLoggedUser.Id))
        {
            _logger.LogWarning(
                "PlaylistId = {Id} associated to userId = {UserId} does not exist",
                playListId,
                currentLoggedUser.Id);
            return Result.NotFound<byte[]>($"PlaylistId = {playListId} does not exist");
        }

        Media? media = await _dataService.Medias.GetById(id);
        if (media == null || media.PlaylistId != playListId)
        {
            _logger.LogWarning("MediaId = {Id} does not exist", id);
            return Result.NotFound<byte[]>($"MediaId = {id} does not exist");
        }

        try
        {
            _logger.LogInformation("Reading media data from path = {Path}...", media.Path);
            byte[] data = await File.ReadAllBytesAsync(media.Path);

            _logger.LogInformation("Got media data from path = {Path}", media.Path);
            return Result.Success(data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unknown error occurred while reading media data from path = {Path}", media.Path);
            throw;
        }
    }

    private async Task<string> SaveMediaData(CreateMediaRequestDto dto)
    {
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "medias");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string ext = Path.GetExtension(dto.Name);
        string filename = $"{Path.GetFileNameWithoutExtension(dto.Name)}_{Guid.NewGuid()}{ext}".ToLowerInvariant();
        string path = Path.Combine(dir, filename);

        await using var fs = new FileStream(path, FileMode.Create);
        dto.Data.Position = 0;
        await dto.Data.CopyToAsync(fs);
        dto.Data.Position = 0;
        return path;
    }
}