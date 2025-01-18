using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.Infrastructure.Persistence.IntegrationTests.Repositories;

[Collection(nameof(DatabaseCollection))]
public class PlaylistRepositoryTests : IAsyncLifetime
{
    private readonly IAppDataService _dataService;
    private readonly DatabaseHelper _databaseHelper;

    public PlaylistRepositoryTests(DatabaseFixture databaseFixture)
    {
        _dataService = databaseFixture.DataService;
        _databaseHelper = databaseFixture.DatabaseHelper;
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _databaseHelper.CleanTablesAsync();
    }

    [Fact]
    public async Task Delete_PlaylistDoesNotExist_ReturnsNormally()
    {
        //Arrange
        const long playListId = 999;

        //Act - Assert
        await Should.NotThrowAsync(() => _dataService.Playlists.Delete(playListId));
    }

    [Fact]
    public async Task Delete_PlaylistExistsAndItIsEmpty_DeletesPlaylist()
    {
        //Arrange
        Playlist playlist = await CreateRandomPlaylist();

        //Act
        await _dataService.Playlists.Delete(playlist.Id);

        //Assert
        bool exists = await _dataService.Playlists.ExistsById(playlist.Id);
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task Delete_PlaylistExistsAndItIsNotEmpty_DeletesPlaylist()
    {
        //Arrange
        Playlist playlist = await CreateRandomPlaylist();
        var media = new Media
        {
            Name = "file.wav",
            Length = 7000,
            Path = "/app/medias/file.wav",
            PlaylistId = playlist.Id,
            LengthInSeconds = 7,
            CreatedAt = DateTime.UtcNow
        };
        await _dataService.Medias.Create(media);
        _databaseHelper.TrackRecord(media);

        //Act
        await _dataService.Playlists.Delete(playlist.Id);

        //Assert
        bool plExists = await _dataService.Playlists.ExistsById(playlist.Id);
        bool mediaExists = await _dataService.Medias.ExistsById(media.Id);
        plExists.ShouldBeFalse();
        mediaExists.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAllByUserId_NoDataExist_ReturnsEmpty()
    {
        //Arrange
        Playlist playlist = await CreateRandomPlaylist();

        //Act
        List<PlaylistResponseDto> playlists = await _dataService.Playlists.GetAllByUserId(playlist.UserId);

        //Assert
        playlists.ShouldNotBeEmpty();
        playlists.ShouldContain(pl => pl.Id == playlist.Id);
    }


    [Fact]
    public async Task GetAllByUserId_DataExists_ReturnsPlaylist()
    {
        //Arrange
        const long userId = 999;

        //Act
        List<PlaylistResponseDto> playlists = await _dataService.Playlists.GetAllByUserId(userId);

        //Assert
        playlists.ShouldBeEmpty();
    }

    private async Task<Playlist> CreateRandomPlaylist()
    {
        var user = new Faker<User>()
            .RuleFor(f => f.Email, f => f.Person.Email)
            .RuleFor(f => f.UserName, (_, dto) => dto.Email)
            .RuleFor(f => f.NormalizedEmail, (_, dto) => dto.Email!.ToUpperInvariant())
            .RuleFor(f => f.NormalizedUserName, (_, dto) => dto.Email!.ToUpperInvariant())
            .RuleFor(f => f.FirstName, f => f.Person.FirstName)
            .RuleFor(f => f.LastName, f => f.Person.LastName)
            .Generate();
        await _dataService.Users.Create(user);
        _databaseHelper.TrackRecord(user);

        var playList = new Faker<Playlist>()
            .RuleFor(f => f.Name, f => f.Random.String2(10))
            .RuleFor(f => f.UserId, user.Id)
            .Generate();
        await _dataService.Playlists.Create(playList);
        _databaseHelper.TrackRecord(playList);
        return playList;
    }
}