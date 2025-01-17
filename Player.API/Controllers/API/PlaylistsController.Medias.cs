using Microsoft.AspNetCore.Mvc;
using Player.API.Authorization;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Medias;
using Player.Domain.Dtos.Responses.Medias;
using Player.Domain.Enums;

namespace Player.API.Controllers.API;

public partial class PlaylistsController
{
    /// <summary>
    /// Gets a list of all the medias on a specific playlist
    /// </summary>
    /// <param name="playListId">The playlist id</param>
    /// <response code="200">The list of     medias</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <response code="404">If something that should exist does not</response>
    /// <returns>The list of medias</returns>
    [HttpGet("{playListId:long}/Medias")]
    [HasMediaPermission(PermissionType.Read)]
    [ProducesResponseType(typeof(ListResultDto<GetAllMediaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(long playListId)
    {
        var result = await _mediasService.GetAllMedias(playListId, _currentLoggedUser);
        return HandleResult(result);
    }

    /// <summary>
    /// Creates a media for on a specific playlist
    /// </summary>
    /// <param name="playListId">The playlist id</param>
    /// <param name="file">The file to upload</param>
    /// <response code="200">The created media</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <response code="404">If something that should exist does not</response>
    /// <returns>The created media</returns>
    [HttpPost("{playListId:long}/Medias")]
    [HasMediaPermission(PermissionType.Create)]
    [ProducesResponseType(typeof(ResultDto<GetAllMediaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(long playListId, IFormFile file)
    {
        await using (var stream = file.OpenReadStream())
        {
            var dto = new CreateMediaRequestDto(file.FileName, stream);
            var result = await _mediasService.CreateMedia(playListId, dto, _currentLoggedUser);
            return HandleResult(result);
        }
    }

    /// <summary>
    /// Creates a media on a specific playlist
    /// </summary>
    /// <param name="playListId">The playlist id</param>
    /// <param name="mediaId">The media id</param>
    /// <response code="200">The result of the operation</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <response code="404">If something that should exist does not</response>
    /// <returns>The result of the operation</returns>
    [HttpDelete("{playListId:long}/Medias/{mediaId:long}")]
    [HasMediaPermission(PermissionType.Delete)]
    [ProducesResponseType(typeof(ResultDto<GetAllMediaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long playListId, long mediaId)
    {
        var result = await _mediasService.DeleteMedia(playListId, mediaId, _currentLoggedUser);
        return HandleResult(result);
    }

    /// <summary>
    /// Gets media's data
    /// </summary>
    /// <param name="playListId">The playlist id</param>
    /// <param name="mediaId">The media id</param>
    /// <response code="200">The media's data</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <response code="404">If something that should exist does not</response>
    /// <returns>The media's data</returns>
    [HttpGet("{playListId:long}/Medias/{mediaId:long}/Data")]
    [HasMediaPermission(PermissionType.Read)]
    [ProducesResponseType(typeof(ResultDto<byte[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetData(long playListId, long mediaId)
    {
        var result = await _mediasService.GetMediaData(playListId, mediaId, _currentLoggedUser);
        return HandleResult(result);
    }
}