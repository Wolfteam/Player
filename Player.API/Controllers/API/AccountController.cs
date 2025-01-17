using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Player.Application.Users;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Users;
using Player.Domain.Entities;

namespace Player.API.Controllers.API;

public class AccountController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserService _userService;

    public AccountController(
        ILoggerFactory loggerFactory,
        UserManager<User> userManager,
        SignInManager<User> signInManager, IUserService userService)
        : base(loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    /// <summary>
    /// Generates a token for the provided user
    /// </summary>
    /// <response code="400">If the request is not valid</response>
    /// <returns>The token</returns>
    [AllowAnonymous]
    [HttpPost(nameof(Token))]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Token([FromBody] LoginRequestDto dto)
    {
        EmptyResultDto loginResult = await _userService.Login(dto);
        if (!loginResult.Succeed)
        {
            return Results.BadRequest(loginResult);
        }

        User user = (await _userManager.FindByEmailAsync(dto.Email))!;
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        return Results.SignIn(claimsPrincipal, authenticationScheme: BearerTokenDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Registers a user
    /// </summary>
    /// <response code="200">The result of the operation</response>
    /// <response code="400">If some of the properties in the request are not valid</response>
    /// <returns>The result of the operation</returns>
    [AllowAnonymous]
    [HttpPost(nameof(Register))]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResultDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDto dto)
    {
        EmptyResultDto result = await _userService.Register(dto);
        return HandleResult(result);
    }
}