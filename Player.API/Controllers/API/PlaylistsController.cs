using Microsoft.AspNetCore.Mvc;
using Player.API.Authorization;
using Player.Application.Medias;
using Player.Application.Playlists;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Playlists;
using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Enums;
using Player.Domain.Interfaces;

namespace Player.API.Controllers.API;

public partial class PlaylistsController : BaseController
{
    private readonly ICurrentLoggedUser _currentLoggedUser;
    private readonly IPlaylistService _playlistService;
    private readonly IMediasService _mediasService;

    public PlaylistsController(
        ILoggerFactory loggerFactory,
        ICurrentLoggedUser currentLoggedUser,
        IPlaylistService playlistService,
        IMediasService mediasService)
        : base(loggerFactory)
    {
        _currentLoggedUser = currentLoggedUser;
        _playlistService = playlistService;
        _mediasService = mediasService;
    }

    /// <summary>
    /// Gets a list of all the playlists
    /// </summary>
    /// <response code="200">The list of playlists</response>
    /// <returns>The list of playlists</returns>
    [HttpGet]
    [HasPlaylistPermission(PermissionType.Read)]
    [ProducesResponseType(typeof(ListResultDto<PlaylistResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _playlistService.GetAllPlaylists(_currentLoggedUser);
        return HandleResult(result);
    }

    /// <summary>
    /// Creates a playlist
    /// </summary>
    /// <param name="dto">The request</param>
    /// <response code="200">The created playlist</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <returns>The created playlist</returns>
    [HttpPost]
    [HasPlaylistPermission(PermissionType.Create)]
    [ProducesResponseType(typeof(ResultDto<PlaylistResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePlaylistRequestDto dto)
    {
        var result = await _playlistService.CreatePlaylist(dto, _currentLoggedUser);
        return HandleResult(result);
    }

    /// <summary>
    /// Updates a playlist
    /// </summary>
    /// <param name="playListId">The playlist id</param>
    /// <param name="dto">The request</param>
    /// <response code="200">The updated playlist</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <response code="404">If something that should exist does not</response>
    /// <returns>The updated playlist</returns>
    [HttpPut("{playListId:long}")]
    [HasPlaylistPermission(PermissionType.Update)]
    [ProducesResponseType(typeof(ResultDto<PlaylistResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long playListId, [FromBody] UpdatePlaylistRequestDto dto)
    {
        var result = await _playlistService.UpdatePlaylist(playListId, dto, _currentLoggedUser);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a playlist and its associated medias
    /// </summary>
    /// <param name="playListId">The playlist id</param>
    /// <response code="200">The result of the operation</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <response code="404">If something that should exist does not</response>
    /// <returns>The result of the operation</returns>
    [HttpDelete("{playListId:long}")]
    [HasPlaylistPermission(PermissionType.Delete)]
    [ProducesResponseType(typeof(ResultDto<PlaylistResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long playListId)
    {
        var result = await _playlistService.DeletePlaylist(playListId, _currentLoggedUser);
        return HandleResult(result);
    }
}