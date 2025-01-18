using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Player.Domain.Dtos;
using Player.Domain.Dtos.Requests.Playlists;
using Player.Domain.Dtos.Requests.Users;
using Player.Domain.Dtos.Responses.Medias;
using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.API.IntegrationTests.API;

[Collection(nameof(WebAppCollection))]
public class PlaylistsControllerTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;

    private const string BasePath = "api/v1";

    private static string PlaylistsPath
        => GetUrlPath("playlists");

    public PlaylistsControllerTests(CustomWebApplicationFactory factory)
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
    public async Task GetAllPlayLists_NoDataExist_ReturnsOk()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();

        //Act
        var response = await client.GetAsync(PlaylistsPath, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ListResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ListResultShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllPlayLists_DataExists_ReturnsOk()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.GetAsync(PlaylistsPath, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ListResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ListResultShouldBeSucceed();
        result!.Result!.ShouldContain(pl => pl.Id == playlist.Id);
    }

    [Fact]
    public async Task CreatePlayList_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePlaylistRequestDto("");
        HttpClient client = await GetAuthorizedHttpClient();

        //Act
        var response = await client.PostAsJsonAsync(PlaylistsPath, dto, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result!.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task CreatePlayList_ValidRequest_ReturnsOk()
    {
        HttpClient client = await GetAuthorizedHttpClient();
        await CreateRandomPlaylist(client);
    }

    [Fact]
    public async Task UpdatePlayList_PlaylistDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var dto = new UpdatePlaylistRequestDto("updated");
        HttpClient client = await GetAuthorizedHttpClient();

        //Act
        var response = await client.PutAsJsonAsync($"{PlaylistsPath}/999", dto, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result!.ResultShouldBeNotFound();
    }

    [Fact]
    public async Task UpdatePlayList_ValidRequest_ReturnsOk()
    {
        // Arrange
        var dto = new UpdatePlaylistRequestDto("updated");
        HttpClient client = await GetAuthorizedHttpClient();
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.PutAsJsonAsync(
            $"{PlaylistsPath}/{playlist.Id}",
            dto,
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ResultShouldBeSucceed();
        result!.Result!.Id.ShouldBe(playlist.Id);
        result.Result.Name.ShouldBe(dto.Name);
    }

    [Fact]
    public async Task DeletePlayList_PlaylistDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();

        //Act
        var response = await client.DeleteAsync($"{PlaylistsPath}/999", TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result!.EmptyResultShouldBeNotFound();
    }

    [Fact]
    public async Task DeletePlayList_ValidRequest_ReturnsOk()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.DeleteAsync(
            $"{PlaylistsPath}/{playlist.Id}",
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.EmptyResultShouldBeSucceed();
    }

    [Fact]
    public async Task GetAllMedias_NoDataExist_ReturnsOk()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.GetAsync(GetMediasPath(playlist.Id), TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ListResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ListResultShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllMedias_PlaylistDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();

        //Act
        var response = await client.GetAsync(GetMediasPath(999), TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ListResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result!.ListResultShouldBeNotFound();
    }

    [Fact]
    public async Task GetAllMedias_DataExists_ReturnsOk()
    {
        // Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        GetAllMediaResponseDto media = await CreateRandomMedia(client);

        //Act
        var response = await client.GetAsync(GetMediasPath(media.PlaylistId), TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ListResultDto<GetAllMediaResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ListResultShouldBeSucceed();
        result!.Result!.ShouldContain(pl => pl.Id == media.Id);
    }

    [Fact]
    public async Task CreateMedia_InvalidRequest_ReturnsBadRequest()
    {
        //Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(Stream.Null), "file", $"{Guid.NewGuid()}.wav");
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.PostAsync(
            GetMediasPath(playlist.Id),
            content,
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<GetAllMediaResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result!.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task CreateMedia_ValidRequest_ReturnsOk()
    {
        HttpClient client = await GetAuthorizedHttpClient();
        await CreateRandomMedia(client);
    }

    [Fact]
    public async Task DeleteMedia_MediaDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.DeleteAsync(
            GetMediasPath(playlist.Id, 999),
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result!.EmptyResultShouldBeNotFound();
    }

    [Fact]
    public async Task DeleteMedia_ValidRequest_ReturnsOk()
    {
        //Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        GetAllMediaResponseDto media = await CreateRandomMedia(client);

        //Act
        var response = await client.DeleteAsync(
            GetMediasPath(media.PlaylistId, media.Id),
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<EmptyResultDto>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.EmptyResultShouldBeSucceed();
    }

    [Fact]
    public async Task GetData_MediaDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.GetAsync(
            $"{GetMediasPath(playlist.Id, 999)}/Data",
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<byte[]>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result!.EmptyResultShouldBeNotFound();
    }

    [Fact]
    public async Task GetData_ValidRequest_ReturnsOk()
    {
        //Arrange
        HttpClient client = await GetAuthorizedHttpClient();
        GetAllMediaResponseDto media = await CreateRandomMedia(client);

        //Act
        var response = await client.GetAsync(
            $"{GetMediasPath(media.PlaylistId, media.Id)}/Data",
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<byte[]>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ResultShouldBeSucceed();
        result!.Result!.ShouldNotBeEmpty();
    }

    private static string GetMediasPath(long playListId, long? mediaId = null)
    {
        string path = $"{PlaylistsPath}/{playListId}/medias";
        if (mediaId.HasValue)
        {
            path += $"/{mediaId.Value}";
        }

        return path;
    }

    private static string GetUrlPath(string suffix)
    {
        return $"{BasePath}/{suffix}";
    }

    private async Task<string> RegisterUserAndGetToken()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var registerDto = new RegisterUserRequestDto(dto.Email, "Nahida", "Kusanali", dto.Password);
        var registerResponse = await client.PostAsJsonAsync(
            GetUrlPath("account/register"),
            registerDto,
            TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();
        using var scope = _factory.Services.CreateScope();
        User? user = await scope.ServiceProvider.GetRequiredService<IAppDataService>().Users.GetByEmail(dto.Email);
        _factory.DatabaseHelper.TrackRecord(user!);

        // Act
        var response = await client.PostAsJsonAsync(
            GetUrlPath("account/token"),
            dto,
            TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<Token>();
        return token!.AccessToken;
    }

    private async Task<HttpClient> GetAuthorizedHttpClient()
    {
        string token = await RegisterUserAndGetToken();
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<PlaylistResponseDto> CreateRandomPlaylist(HttpClient client)
    {
        // Arrange
        var dto = new Faker<CreatePlaylistRequestDto>()
            .CustomInstantiator(f => new CreatePlaylistRequestDto(f.Random.String2(10)))
            .Generate();

        //Act
        var response = await client.PostAsJsonAsync(PlaylistsPath, dto, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<PlaylistResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ResultShouldBeSucceed();
        using var scope = _factory.Services.CreateScope();
        Playlist? playlist = await scope.ServiceProvider
            .GetRequiredService<IAppDataService>().Playlists
            .GetById(result!.Result!.Id);
        _factory.DatabaseHelper.TrackRecord(playlist!);

        return result.Result!;
    }

    private async Task<GetAllMediaResponseDto> CreateRandomMedia(HttpClient client)
    {
        //Arrange
        using var content = new MultipartFormDataContent();
        string validWavFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "file.wav");
        FileStream fileStream = File.Open(validWavFile, FileMode.Open);
        content.Add(new StreamContent(fileStream), "file", $"{Guid.NewGuid()}.wav");
        PlaylistResponseDto playlist = await CreateRandomPlaylist(client);

        //Act
        var response = await client.PostAsync(
            $"{PlaylistsPath}/{playlist.Id}/Medias",
            content,
            TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResultDto<GetAllMediaResponseDto>>(
            cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        response.EnsureSuccessStatusCode();
        result!.ResultShouldBeSucceed();
        using var scope = _factory.Services.CreateScope();
        Media? media = await scope.ServiceProvider
            .GetRequiredService<IAppDataService>().Medias
            .GetById(result!.Result!.Id);
        _factory.DatabaseHelper.TrackRecord(media!);

        return result.Result!;
    }
}

public class Token
{
    public string TokenType { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;
}