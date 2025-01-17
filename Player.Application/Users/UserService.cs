using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Player.Application.Validation;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Users;
using Player.Domain.Entities;
using Player.Domain.Enums;

namespace Player.Application.Users;

internal class UserService : IUserService
{
    private readonly ILogger _logger;
    private readonly IValidatorService _validator;
    private readonly UserManager<User> _userManager;

    public UserService(
        ILoggerFactory loggerFactory,
        IValidatorService validator,
        UserManager<User> userManager)
    {
        _logger = loggerFactory.CreateLogger(GetType());
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<EmptyResultDto> Register(RegisterUserRequestDto dto)
    {
        EmptyResultDto validationResult = _validator.Validate(dto);
        if (!validationResult.Succeed)
        {
            return validationResult;
        }

        _logger.LogInformation("Checking if user = {Email} already exists...", dto.Email);
        User? existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("A user with email = {Email} already exists", dto.Email);
            return EmptyResult.ResourceAlreadyExists("User already exists");
        }

        _logger.LogInformation("Creating user = {Email}...", dto.Email);
        var user = new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = true
        };
        IdentityResult result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to create user = {Email}. Result = {@Result}", dto.Email, result);
            return EmptyResult.UnknownError("Could not create user");
        }

        _logger.LogInformation("User = {Email} was successfully created...", dto.Email);
        return EmptyResult.Success();
    }

    public async Task<EmptyResultDto> Login(LoginRequestDto dto)
    {
        EmptyResultDto validationResult = _validator.Validate(dto);
        if (!validationResult.Succeed)
        {
            return validationResult;
        }

        _logger.LogInformation("Getting user = {Email}...", dto.Email);
        User? user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            _logger.LogWarning("User = {Email} does not exist", dto.Email);
            return EmptyResult.NotFound("User does not exist");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("User = {Email} is locked until = {Until}", dto.Email, user.LockoutEnd);
            return EmptyResult.InvalidRequest(AppMessageType.UserIsLockedOut, "User is locked");
        }

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            await _userManager.AccessFailedAsync(user);
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User = {Email} is locked out", dto.Email);
                return EmptyResult.InvalidRequest(AppMessageType.UserIsLockedOut, "User is locked out");
            }

            _logger.LogWarning("Provided password for user = {Email} is not valid", dto.Email);
            return EmptyResult.NotFound("User or password is not valid");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        _logger.LogInformation("Login succeed for user = {Email}", dto.Email);
        return EmptyResult.Success();
    }
}