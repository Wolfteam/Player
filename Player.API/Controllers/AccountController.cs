using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Player.API.ViewModels;
using Player.Application.Users;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Users;
using Player.Domain.Entities;
using Player.Domain.Enums;

namespace Player.API.Controllers;

[AllowAnonymous]
public class AccountController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserService _userService;

    public AccountController(
        ILoggerFactory loggerFactory,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserService userService)
        : base(loggerFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        SetReturnUrl(returnUrl);
        if (User.Identity?.IsAuthenticated == true)
        {
            return Redirect("~/");
        }

        return View(LoginViewModel.FromReturnUrl(returnUrl));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        SetReturnUrl(vm.ReturnUrl);

        if (!ModelState.IsValid)
            return View(vm);

        Logger.LogInformation("Trying to login user = {Email}", vm.Email);
        var dto = new LoginRequestDto(vm.Email ?? string.Empty, vm.Password ?? string.Empty);
        EmptyResultDto loginResult = await _userService.Login(dto);
        if (!loginResult.Succeed)
        {
            Logger.LogWarning("Auth failed for user = {Email}. Error = {Error}", dto.Email, loginResult.Message);
            ModelState.AddModelError(
                string.Empty,
                loginResult.MessageType == AppMessageType.UserIsLockedOut
                    ? "You are locked out. Please try again later"
                    : "Please check your credentials");
            return View(vm);
        }

        User user = (await _userManager.FindByEmailAsync(dto.Email))!;
        var signInResult = await _signInManager.PasswordSignInAsync(user, dto.Password, true, true);
        if (signInResult.Succeeded)
        {
            if (Url.IsLocalUrl(vm.ReturnUrl))
            {
                return Redirect(vm.ReturnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        Logger.LogWarning("Auth failed for user = {Email}. Result = {@Result}", dto.Email, signInResult);
        ModelState.AddModelError(string.Empty, "Please check your credentials");
        return View(vm);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        SetReturnUrl(returnUrl);
        if (User.Identity?.IsAuthenticated == true)
        {
            return Redirect("~/");
        }

        return View(RegisterViewModel.FromReturnUrl(returnUrl));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        SetReturnUrl(vm.ReturnUrl);

        if (!ModelState.IsValid)
            return View(vm);

        Logger.LogInformation("Trying to register user = {Email}", vm.Email);
        var dto = new RegisterUserRequestDto(
            vm.Email ?? string.Empty,
            vm.FirstName ?? string.Empty,
            vm.LastName ?? string.Empty,
            vm.Password ?? string.Empty);
        EmptyResultDto result = await _userService.Register(dto);
        if (result.Succeed)
        {
            Logger.LogInformation("User = {Email} was successfully registered", dto.Email);
            vm.Registered = true;
            return View(vm);
        }

        if (result.MessageType == AppMessageType.ResourceAlreadyExists)
        {
            ModelState.AddModelError(string.Empty, "User already exists");
        }

        Logger.LogWarning("User = {Email} registration failed. Error = {Error}", dto.Email, result.Message);
        ModelState.AddModelError(string.Empty, "Invalid request");
        return View(vm);
    }

    public async Task<IActionResult> Logout()
    {
        Logger.LogInformation("Logging out user = {User} ...", HttpContext.User.Identity?.Name);
        await _signInManager.SignOutAsync();
        Logger.LogInformation("Logout completed...");
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private void SetReturnUrl(string? url)
    {
        ViewData["ReturnUrl"] = url;
    }
}