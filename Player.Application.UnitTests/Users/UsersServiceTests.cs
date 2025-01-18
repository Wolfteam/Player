using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Player.Application.Users;
using Player.Domain.Dtos.Requests.Users;
using Player.Domain.Entities;
using Player.Domain.Enums;

namespace Player.Application.UnitTests.Users;

public class UsersServiceTests : BaseTests
{
    [Fact]
    public async Task Register_InvalidRequestDto_ReturnsInvalidRequest()
    {
        //Arrange
        var dto = new RegisterUserRequestDto("nahida", "Nahida", "Kusanali", "qwerty1234");
        IUserService sut = GetService();

        //Act
        var result = await sut.Register(dto);

        //Assert
        result.EmptyResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task Register_UserAlreadyExists_ReturnsResourceAlreadyExists()
    {
        //Arrange
        var dto = new RegisterUserRequestDto("nahida@saikoudesu.com", "Nahida", "Kusanali", "qwerty1234");
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Email = dto.Email
            });
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Register(dto);

        //Assert
        result.EmptyResultShouldFail(AppMessageType.ResourceAlreadyExists);
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_UserFailsToBeCreated_ReturnsUnknownError()
    {
        //Arrange
        var dto = new RegisterUserRequestDto("nahida@saikoudesu.com", "Nahida", "Kusanali", "qwerty1234");
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);
        userStoreMock.Setup(mock => mock.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed());
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Register(dto);

        //Assert
        result.EmptyResultShouldBeUnknownError();
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        var dto = new RegisterUserRequestDto("nahida@saikoudesu.com", "Nahida", "Kusanali", "qwerty1234");
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);
        userStoreMock.Setup(mock => mock.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Register(dto);

        //Assert
        result.EmptyResultShouldBeSucceed();
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_InvalidRequestDto_ReturnsInvalidRequest()
    {
        //Arrange
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "");
        IUserService sut = GetService();

        //Act
        var result = await sut.Login(dto);

        //Assert
        result.EmptyResultShouldBeInvalidRequest();
    }

    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Login(dto);

        //Assert
        result.EmptyResultShouldBeNotFound();
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_UserIsLockedOut_ReturnsUserIsLockedOut()
    {
        //Arrange
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var user = new User
        {
            Email = dto.Email
        };
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userStoreMock.Setup(mock => mock.GetLockoutEnabledAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        userStoreMock.Setup(mock => mock.GetLockoutEndDateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DateTimeOffset.UtcNow.AddMinutes(15));
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Login(dto);

        //Assert
        result.EmptyResultShouldFail(AppMessageType.UserIsLockedOut);
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.GetLockoutEnabledAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.GetLockoutEndDateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_PasswordIsNotValid_ReturnsInvalidRequest()
    {
        //Arrange
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var user = new User
        {
            Email = dto.Email
        };
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userStoreMock.Setup(mock => mock.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync("notvalid");
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Login(dto);

        //Assert
        result.EmptyResultShouldBeNotFound();
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_PasswordIsNotValidAndUserIsLockedOut_ReturnsUserIsLockedOut()
    {
        //Arrange
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var user = new User
        {
            Email = dto.Email
        };
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        userStoreMock.Setup(mock => mock.GetLockoutEnabledAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        userStoreMock.SetupSequence(mock => mock.GetLockoutEndDateAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null)
            .ReturnsAsync(DateTimeOffset.UtcNow.AddMinutes(15));
        userStoreMock.Setup(mock => mock.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync("notvalid");
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Login(dto);

        //Assert
        result.EmptyResultShouldFail(AppMessageType.UserIsLockedOut);
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.GetLockoutEnabledAsync(user, It.IsAny<CancellationToken>()), Times.Exactly(2));
        userStoreMock.Verify(mock => mock.GetLockoutEndDateAsync(user, It.IsAny<CancellationToken>()), Times.Exactly(2));
        userStoreMock.Verify(mock => mock.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_ValidRequest_ReturnsValidResponse()
    {
        //Arrange
        var dto = new LoginRequestDto("nahida@saikoudesu.com", "qwerty1234");
        var user = new User
        {
            Email = dto.Email,
            AccessFailedCount = 2,
            PasswordHash = new PasswordHasher<User>().HashPassword(null, dto.Password)
        };
        var userStoreMock = new Mock<TestUserStore>();
        userStoreMock.Setup(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        userStoreMock.Setup(mock => mock.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user.PasswordHash);
        UserManager<User> userManagerMock = GetMockedUserManager(userStoreMock);
        IUserService sut = GetService(userManagerMock);

        //Act
        var result = await sut.Login(dto);

        //Assert
        result.EmptyResultShouldBeSucceed();
        userStoreMock.Verify(mock => mock.FindByEmailAsync(dto.Email, It.IsAny<CancellationToken>()), Times.Once);
        userStoreMock.Verify(mock => mock.GetPasswordHashAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static IUserService GetService(UserManager<User>? userManager = null)
    {
        return BuildServices((_, sp) =>
        {
            sp.AddUserService();
            sp.AddScoped(_ => userManager ?? GetMockedUserManager());
        }).GetRequiredService<IUserService>();
    }

    private static UserManager<User> GetMockedUserManager(Mock<TestUserStore>? store = null)
    {
        var userManager = new UserManager<User>(
            store?.Object ?? new Mock<TestUserStore>().Object, null,
            new PasswordHasher<User>(), null,
            null, null, null, null,
            new Mock<ILogger<UserManager<User>>>().Object);
        return userManager;
    }
}