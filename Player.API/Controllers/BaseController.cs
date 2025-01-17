using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Player.Domain.Dtos;
using Player.Domain.Enums;

namespace Player.API.Controllers;

[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class BaseController : Controller
{
    protected readonly ILogger Logger;

    protected BaseController(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    public IActionResult HandleResult<T>(T result) where T : EmptyResultDto
    {
        if (result.Succeed)
        {
            return Ok(result);
        }

        return result.MessageType switch
        {
            AppMessageType.UnknownError => StatusCode(StatusCodes.Status500InternalServerError, result),
            AppMessageType.InvalidRequest or
                AppMessageType.ResourceAlreadyExists => BadRequest(result),
            AppMessageType.NotFound => NotFound(result),
            AppMessageType.UserIsLockedOut => Unauthorized(result),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}