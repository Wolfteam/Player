using Microsoft.AspNetCore.Mvc;
using Player.API.ViewModels;
using Player.Application.Medias;
using Player.Application.Playlists;
using Player.Domain.Dtos.Requests.Medias;
using Player.Domain.Dtos.Requests.Playlists;
using Player.Domain.Interfaces;
using ZstdSharp.Unsafe;

namespace Player.API.Controllers;

public class HomeController : BaseController
{
    private readonly ICurrentLoggedUser _currentLoggedUser;
    private readonly IPlaylistService _playlistService;
    private readonly IMediasService _mediasService;

    public HomeController(
        ILoggerFactory loggerFactory,
        ICurrentLoggedUser currentLoggedUser,
        IPlaylistService playlistService,
        IMediasService mediasService)
        : base(loggerFactory)
    {
        _playlistService = playlistService;
        _mediasService = mediasService;
        _currentLoggedUser = currentLoggedUser;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _playlistService.GetAllPlaylists(_currentLoggedUser);
        if (!result.Succeed)
        {
            return View(new HomeViewModel(new PlaylistsViewModel([]))
            {
                UnknownError = true
            });
        }

        var playlists = result.Result!.ConvertAll(pl => new PlaylistViewModel(pl.Id, pl.Name, pl.MediaCount));
        return View(new HomeViewModel(new PlaylistsViewModel(playlists)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlaylist(PlaylistViewModel vm)
    {
        var result = await _playlistService.CreatePlaylist(new CreatePlaylistRequestDto(vm.Name), _currentLoggedUser);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePlaylist(PlaylistViewModel vm)
    {
        var result = await _playlistService.UpdatePlaylist(
            vm.Id,
            new UpdatePlaylistRequestDto(vm.Name),
            _currentLoggedUser);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePlaylist(long id)
    {
        await _playlistService.DeletePlaylist(id, _currentLoggedUser);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedia(long playListId, IFormFile file)
    {
        await using (var stream = file.OpenReadStream())
        {
            var dto = new CreateMediaRequestDto(file.FileName, stream);
            var result = await _mediasService.CreateMedia(playListId, dto, _currentLoggedUser);
            return HandleResult(result);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMedia(long playListId)
    {
        var result = await _mediasService.GetAllMedias(playListId, _currentLoggedUser);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMedia(long playListId, long mediaId)
    {
        await _mediasService.DeleteMedia(playListId, mediaId, _currentLoggedUser);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> GetMediaData(long playListId, long mediaId)
    {
        var result = await _mediasService.GetMediaData(playListId, mediaId, _currentLoggedUser);
        return File(result.Result ?? [], "audio/wav", $"{playListId}_{mediaId}.wav");
    }
}