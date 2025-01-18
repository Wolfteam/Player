using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.Infrastructure.Persistence.IntegrationTests.Repositories;

[Collection(nameof(DatabaseCollection))]
public class MediaRepositoryTests : IAsyncLifetime
{
    private readonly IAppDataService _dataService;
    private readonly DatabaseHelper _databaseHelper;

    public MediaRepositoryTests(DatabaseFixture databaseFixture)
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
    public async Task ExistsByIdAndPlayListId_NoDataExist_ReturnsFalse()
    {
        //Arrange
        const long mediaId = 666;
        const long playlistId = 999;

        //Act
        bool exists = await _dataService.Medias.ExistsByIdAndPlayListId(mediaId, playlistId);

        //Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task ExistsByIdAndPlayListId_DataExists_ReturnsTrue()
    {
        //Arrange
        Playlist playList = await CreateRandomPlaylist();

        var media = new Media
        {
            Name = "file.wav",
            Length = 7000,
            Path = "/app/medias/file.wav",
            PlaylistId = playList.Id,
            LengthInSeconds = 7,
            CreatedAt = DateTime.UtcNow
        };
        await _dataService.Medias.Create(media);
        _databaseHelper.TrackRecord(media);

        //Act
        bool exists = await _dataService.Medias.ExistsByIdAndPlayListId(media.Id, playList.Id);

        //Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllByPlayListId_NoDataExists_ReturnsEmpty()
    {
        //Arrange
        const long playlistId = 999;

        //Act
        List<Media> results = await _dataService.Medias.GetAllByPlayListId(playlistId);

        //Assert
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllByPlayListId_DataExists_ReturnsItems()
    {
        //Arrange
        Playlist playList = await CreateRandomPlaylist();

        var mediaA = new Media
        {
            Name = "fileb.wav",
            Length = 7000,
            Path = "/app/medias/fileb.wav",
            PlaylistId = playList.Id,
            LengthInSeconds = 7,
            CreatedAt = DateTime.UtcNow
        };
        await _dataService.Medias.Create(mediaA);
        _databaseHelper.TrackRecord(mediaA);

        var mediaB = new Media
        {
            Name = "filea.wav",
            Length = 5000,
            Path = "/app/medias/file.wav",
            PlaylistId = playList.Id,
            LengthInSeconds = 3,
            CreatedAt = DateTime.UtcNow
        };
        await _dataService.Medias.Create(mediaB);
        _databaseHelper.TrackRecord(mediaB);

        //Act
        List<Media> results = await _dataService.Medias.GetAllByPlayListId(playList.Id);

        //Assert
        results.Count.ShouldBe(2);
        results.ShouldContain(m => m.Id == mediaA.Id);
        results.ShouldContain(m => m.Id == mediaB.Id);
    }

    [Fact]
    public async Task GetMediaCountByPlayListId_NoDataExists_ReturnsEmpty()
    {
        //Arrange
        const long playListId = 999;

        //Act
        long count  = await _dataService.Medias.GetMediaCountByPlayListId(playListId);

        //Assert
        count.ShouldBe(0);
    }

    [Fact]
    public async Task GetMediaCountByPlayListId_DataExists_ReturnsGreaterThanZero()
    {
        //Arrange
        Playlist playList = await CreateRandomPlaylist();

        var mediaA = new Media
        {
            Name = "fileb.wav",
            Length = 7000,
            Path = "/app/medias/fileb.wav",
            PlaylistId = playList.Id,
            LengthInSeconds = 7,
            CreatedAt = DateTime.UtcNow
        };
        await _dataService.Medias.Create(mediaA);
        _databaseHelper.TrackRecord(mediaA);

        var mediaB = new Media
        {
            Name = "filea.wav",
            Length = 5000,
            Path = "/app/medias/file.wav",
            PlaylistId = playList.Id,
            LengthInSeconds = 3,
            CreatedAt = DateTime.UtcNow
        };
        await _dataService.Medias.Create(mediaB);
        _databaseHelper.TrackRecord(mediaB);

        //Act
        long count  = await _dataService.Medias.GetMediaCountByPlayListId(playList.Id);

        //Assert
        count.ShouldBe(2);
    }

    private async Task<Playlist> CreateRandomPlaylist()
    {
        var user = new Faker<User>()
            .RuleFor(f => f.Email, f => f.Person.Email)
            .RuleFor(f => f.UserName, (f, dto) => dto.Email)
            .RuleFor(f => f.NormalizedEmail, (f, dto) => dto.Email!.ToUpperInvariant())
            .RuleFor(f => f.NormalizedUserName, (f, dto) => dto.Email!.ToUpperInvariant())
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