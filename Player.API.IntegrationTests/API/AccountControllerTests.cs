using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Users;
using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.API.IntegrationTests.API;

[Collection(nameof(WebAppCollection))]
public class AccountControllerTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;

    private static string RegisterPath
        => GetUrlPath("register");

    private static string TokenPath
        => GetUrlPath("token");

    public AccountControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }


    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DatabaseHelper.CleanTablesAsync();
    }

    [Fact]
    public async Task Token_UserDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        var dto = new LoginRequestDto("notvalid@saikoudesu.com", "qwerty1234");

        // Act
        var response = await client.PostAsJsonAsync(
            TokenPath,
            dto,
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result!.EmptyResultShouldBeNotFound();
    }

    [Fact]
    public async Task Token_UserExists_ReturnsOk()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var registerDto = new RegisterUserRequestDto(dto.Email, "Nahida", "Kusanali", dto.Password);
        var registerResponse = await client.PostAsJsonAsync(
            RegisterPath,
            registerDto,
            TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();
        await TrackUser(dto.Email);

        // Act
        var response = await client.PostAsJsonAsync(
            TokenPath,
            dto,
            TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Register_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        var registerDto = new RegisterUserRequestDto("notvalid", "Nahida", "Kusanali", "qwerty1234");

        // Act
        var response = await client.PostAsJsonAsync(
            RegisterPath,
            registerDto,
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result!.EmptyResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        var dto = new RegisterUserRequestDto("nahida@saikoudesu.com", "Nahida", "Kusanali", "qwerty1234");

        // Act
        var response = await client.PostAsJsonAsync(
            RegisterPath,
            dto,
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        result!.EmptyResultShouldBeSucceed();
        await TrackUser(dto.Email);
    }

    private async Task TrackUser(string email)
    {
        using var scope = _factory.Services.CreateScope();
        User? user = await scope.ServiceProvider.GetRequiredService<IAppDataService>().Users.GetByEmail(email);
        _factory.DatabaseHelper.TrackRecord(user!);
    }

    private static string GetUrlPath(string suffix)
    {
        return $"api/v1/account/{suffix}";
    }
}