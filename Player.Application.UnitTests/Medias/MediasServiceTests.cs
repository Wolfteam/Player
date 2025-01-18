using Microsoft.Extensions.DependencyInjection;
using Player.Application.Medias;
using Player.Domain.Dtos.Requests.Medias;
using Player.Domain.Dtos.Responses.Medias;
using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.Application.UnitTests.Medias;

public class MediasServiceTests : BaseTests
{
    private static readonly string ValidWavFile = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Resources",
        "file.wav");

    [Fact]
    public async Task GetAllMedias_InvalidPlayListId_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = 0;
        IMediasService sut = GetService();

        //Act
        var result = await sut.GetAllMedias(playListId, new TestCurrentUser());

        //Assert
        result.ListResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task GetAllMedias_PlaylistDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(false);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.GetAllMedias(playListId, new TestCurrentUser());

        //Assert
        result.ListResultShouldBeNotFound();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
    }

    [Fact]
    public async Task GetAllMedias_DataExists_ReturnsValidResponse()
    {
        //Arrange
        const long playListId = 100;
        var user = new TestCurrentUser();
        var medias = new Faker<Media>()
            .RuleFor(f => f.Id, f => f.Random.Long(1))
            .RuleFor(f => f.PlaylistId, playListId)
            .RuleFor(f => f.Name, f => f.System.FileName())
            .RuleFor(f => f.Length, f => f.Random.Long(1))
            .RuleFor(f => f.LengthInSeconds, f => f.Random.Float(1))
            .Generate(10);
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        dataService.Setup(mock => mock.Medias.GetAllByPlayListId(playListId))
            .ReturnsAsync(medias);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.GetAllMedias(playListId, user);

        //Assert
        result.ListResultShouldBeSucceed();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataService.Verify(mock => mock.Medias.GetAllByPlayListId(playListId), Times.Once);

        for (int i = 0; i < result.Result!.Count; i++)
        {
            GetAllMediaResponseDto got = result.Result[i];
            Media expected = medias[i];
            got.Id.ShouldBe(expected.Id);
            got.PlaylistId.ShouldBe(expected.PlaylistId);
            got.Name.ShouldBe(expected.Name);
            got.Length.ShouldBe(expected.Length);
            got.Duration.ShouldBe(expected.LengthInSeconds);
        }
    }

    [Fact]
    public async Task CreateMedia_InvalidPlayListId_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = -1;
        var dto = new CreateMediaRequestDto("file.wav", Stream.Null);
        var user = new TestCurrentUser();
        IMediasService sut = GetService();

        //Act
        var result = await sut.CreateMedia(playListId, dto, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task CreateMedia_InvalidRequestDto_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = 100;
        var dto = new CreateMediaRequestDto("file.wav", Stream.Null);
        var user = new TestCurrentUser();
        IMediasService sut = GetService();

        //Act
        var result = await sut.CreateMedia(playListId, dto, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task CreateMedia_PlaylistDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        await using var fs = new FileStream(ValidWavFile, FileMode.Open);
        var dto = new CreateMediaRequestDto("file.wav", fs);
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(false);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.CreateMedia(playListId, dto, user);

        //Assert
        result.ResultShouldBeNotFound();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
    }

    [Fact]
    public async Task CreateMedia_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        const long playListId = 100;
        await using var fs = new FileStream(ValidWavFile, FileMode.Open);
        var dto = new CreateMediaRequestDto("file.wav", fs);
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        dataService.Setup(mock => mock.Medias.Create(It.IsAny<Media>()))
            .Callback<Media>(entity => entity.Id++);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.CreateMedia(playListId, dto, user);

        //Assert
        result.ResultShouldBeSucceed();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataService.Verify(mock => mock.Medias.Create(It.IsAny<Media>()), Times.Once);

        GetAllMediaResponseDto media = result.Result!;
        media.Id.ShouldBeGreaterThan(0);
        media.PlaylistId.ShouldBe(playListId);
        media.Name.ShouldBe(dto.Name);
        media.Length.ShouldBeGreaterThan(0);
        media.Duration.ShouldBeGreaterThan(5);
    }

    [Fact]
    public async Task DeleteMedia_InvalidPlayListId_ReturnInvalidRequest()
    {
        //Arrange
        const long playListId = -100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        IMediasService sut = GetService();

        //Act
        var result = await sut.DeleteMedia(playListId, mediaId, user);

        //Assert
        result.EmptyResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task DeleteMedia_InvalidMediaId_ReturnInvalidRequest()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = -101;
        var user = new TestCurrentUser();
        IMediasService sut = GetService();

        //Act
        var result = await sut.DeleteMedia(playListId, mediaId, user);

        //Assert
        result.EmptyResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task DeleteMedia_PlaylistDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(false);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.DeleteMedia(playListId, mediaId, user);

        //Assert
        result.EmptyResultShouldBeNotFound();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
    }

    [Fact]
    public async Task DeleteMedia_MediaDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        dataService.Setup(mock => mock.Medias.GetById(mediaId))
            .ReturnsAsync(() => null);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.DeleteMedia(playListId, mediaId, user);

        //Assert
        result.EmptyResultShouldBeNotFound();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataService.Verify(mock => mock.Medias.GetById(mediaId), Times.Once);
    }

    [Fact]
    public async Task DeleteMedia_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        dataService.Setup(mock => mock.Medias.GetById(mediaId))
            .ReturnsAsync(new Media
            {
                Id = mediaId,
                PlaylistId = playListId,
                Path = "dummy.wav"
            });
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.DeleteMedia(playListId, mediaId, user);

        //Assert
        result.EmptyResultShouldBeSucceed();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataService.Verify(mock => mock.Medias.GetById(mediaId), Times.Once);
        dataService.Verify(mock => mock.Medias.Delete(mediaId), Times.Once);
    }

    [Fact]
    public async Task GetMediaData_InvalidPlaylistId_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = -100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        IMediasService sut = GetService();

        //Act
        var result = await sut.GetMediaData(playListId, mediaId, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }


    [Fact]
    public async Task GetMediaData_InvalidMediaId_ReturnsInvalidRequest()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = -101;
        var user = new TestCurrentUser();
        IMediasService sut = GetService();

        //Act
        var result = await sut.GetMediaData(playListId, mediaId, user);

        //Assert
        result.ResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task GetMediaData_PlaylistDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(false);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.GetMediaData(playListId, mediaId, user);

        //Assert
        result.ResultShouldBeNotFound();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
    }

    [Fact]
    public async Task GetMediaData_MediaDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        dataService.Setup(mock => mock.Medias.GetById(mediaId))
            .ReturnsAsync(() => null);
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.GetMediaData(playListId, mediaId, user);

        //Assert
        result.ResultShouldBeNotFound();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataService.Verify(mock => mock.Medias.GetById(mediaId), Times.Once);
    }

    [Fact]
    public async Task GetMediaData_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        const long playListId = 100;
        const long mediaId = 101;
        var user = new TestCurrentUser();
        var dataService = new Mock<IAppDataService>();
        dataService.Setup(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id))
            .ReturnsAsync(true);
        dataService.Setup(mock => mock.Medias.GetById(mediaId))
            .ReturnsAsync(new Media
            {
                Id = mediaId,
                PlaylistId = playListId,
                Path = ValidWavFile
            });
        IMediasService sut = GetService(dataService.Object);

        //Act
        var result = await sut.GetMediaData(playListId, mediaId, user);

        //Assert
        result.ResultShouldBeSucceed();
        dataService.Verify(mock => mock.Playlists.ExistsByIdAndUserId(playListId, user.Id), Times.Once);
        dataService.Verify(mock => mock.Medias.GetById(mediaId), Times.Once);
        result.Result.ShouldNotBeEmpty();
    }

    private static IMediasService GetService(IAppDataService? dataService = null)
    {
        return BuildServices((_, sp) =>
        {
            sp.AddMediaService();
            sp.AddScoped(_ => dataService ?? Mock.Of<IAppDataService>());
        }).GetRequiredService<IMediasService>();
    }
}