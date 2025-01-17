using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Player.Domain.Dtos;
using Player.Domain.Enums;

namespace Player.API.Controllers.API;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = BearerTokenDefaults.AuthenticationScheme)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status500InternalServerError)]
public class BaseController : ControllerBase
{
    protected readonly ILogger Logger;

    protected BaseController(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    protected IActionResult HandleResult<T>(T result) where T : EmptyResultDto
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