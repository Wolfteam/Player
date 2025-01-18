using Microsoft.Extensions.DependencyInjection;
using Player.Application.Playlists;
using Player.Domain.Dtos.Requests.Playlists;
using Player.Domain.Dtos.Responses.Playlists;
using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.Application.UnitTests.Playlists;

public class PlaylistServiceTests : BaseTests
{
    [Fact]
    public async Task GetAllPlaylists_NoDataExist_ReturnsValidResponse()
    {
        //Arrange
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.GetAllByUserId(user.Id))
            .ReturnsAsync([]);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.GetAllPlaylists(user);

        //Assert
        result.ListResultShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllPlaylists_DataExists_ReturnsValidResponse()
    {
        //Arrange
        var data = new Faker<PlaylistResponseDto>()
            .CustomInstantiator(f => new PlaylistResponseDto(f.Random.Long(1), f.Random.String2(5), f.Random.Long(0)))
            .Generate(10);
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.GetAllByUserId(user.Id))
            .ReturnsAsync(data);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.GetAllPlaylists(user);

        //Assert
        result.ListResultShouldBeSucceed();
        result.Result.ShouldBe(data);
    }

    [Fact]
    public async Task CreatePlaylist_InvalidRequestDto_ReturnsInvalidRequest()
    {
        //Arrange
        var dto = new CreatePlaylistRequestDto("");
        var user = new TestCurrentUser();
        IPlaylistService sut = GetService();

        //Act
        var result = await sut.CreatePlaylist(dto, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task CreatePlaylist_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        var dto = new CreatePlaylistRequestDto("NahidaSaikou");
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.Create(It.IsAny<Playlist>()))
            .Callback<Playlist>(entity => entity.Id++);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.CreatePlaylist(dto, user);

        //Assert
        result.ResultShouldBeSucceed();
        dataServiceMock.Verify(mock => mock.Playlists.Create(It.IsAny<Playlist>()), Times.Once);
        PlaylistResponseDto playlist = result.Result!;
        playlist.Id.ShouldBeGreaterThan(0);
        playlist.Name.ShouldBe(dto.Name);
        playlist.MediaCount.ShouldBe(0);
    }

    [Fact]
    public async Task UpdatePlaylist_InvalidPlaylistId_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = -100;
        var dto = new UpdatePlaylistRequestDto("");
        var user = new TestCurrentUser();
        IPlaylistService sut = GetService();

        //Act
        var result = await sut.UpdatePlaylist(playListId, dto, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task UpdatePlaylist_InvalidRequestDto_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = 100;
        var dto = new UpdatePlaylistRequestDto("");
        var user = new TestCurrentUser();
        IPlaylistService sut = GetService();

        //Act
        var result = await sut.UpdatePlaylist(playListId, dto, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task UpdatePlaylist_PlaylistDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        var dto = new UpdatePlaylistRequestDto("NahidaSaikou");
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.GetById(playListId))
            .ReturnsAsync(() => null);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.UpdatePlaylist(playListId, dto, user);

        //Assert
        result.ResultShouldBeNotFound();
        dataServiceMock.Verify(mock => mock.Playlists.GetById(playListId), Times.Once);
    }

    [Fact]
    public async Task UpdatePlaylist_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        const long playListId = 100;
        const long mediaCount = 3;
        var dto = new UpdatePlaylistRequestDto("NahidaSaikou");
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.GetById(playListId))
            .ReturnsAsync(new Playlist
            {
                Id = playListId,
                UserId = user.Id,
                Name = "Something"
            });
        dataServiceMock.Setup(mock => mock.Medias.GetMediaCountByPlayListId(playListId))
            .ReturnsAsync(mediaCount);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.UpdatePlaylist(playListId, dto, user);

        //Assert
        result.ResultShouldBeSucceed();
        dataServiceMock.Verify(mock => mock.Playlists.GetById(playListId), Times.Once);
        dataServiceMock.Verify(mock => mock.Playlists.Update(It.Is<Playlist>(x => x.Id == playListId)), Times.Once);
        PlaylistResponseDto playlist = result.Result!;
        playlist.Id.ShouldBe(playListId);
        playlist.Name.ShouldBe(dto.Name);
        playlist.MediaCount.ShouldBe(mediaCount);
    }

    [Fact]
    public async Task DeletePlaylist_InvalidPlaylistId_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = -100;
        var user = new TestCurrentUser();
        IPlaylistService sut = GetService();

        //Act
        var result = await sut.DeletePlaylist(playListId, user);

        //Assert
        result.EmptyResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task DeletePlaylist_PlaylistDoesNotFound_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(false);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.DeletePlaylist(playListId, user);

        //Assert
        result.EmptyResultShouldBeNotFound();
        dataServiceMock.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
    }

    [Fact]
    public async Task DeletePlaylist_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        const long playListId = 100;
        var user = new TestCurrentUser();
        var dataServiceMock = new Mock<IAppDataService>();
        dataServiceMock.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        IPlaylistService sut = GetService(dataServiceMock.Object);

        //Act
        var result = await sut.DeletePlaylist(playListId, user);

        //Assert
        result.EmptyResultShouldBeSucceed();
        dataServiceMock.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataServiceMock.Verify(mock => mock.Playlists.Delete(playListId), Times.Once);
    }

    private static IPlaylistService GetService(IAppDataService? dataService = null)
    {
        return BuildServices((_, sp) =>
        {
            sp.AddPlaylistService();
            sp.AddScoped(_ => dataService ?? Mock.Of<IAppDataService>());
        }).GetRequiredService<IPlaylistService>();
    }
}