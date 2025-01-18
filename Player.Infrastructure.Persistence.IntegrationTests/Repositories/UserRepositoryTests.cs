using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;

namespace Player.Infrastructure.Persistence.IntegrationTests.Repositories;

[Collection(nameof(DatabaseCollection))]
public class UserRepositoryTests: IAsyncLifetime
{
    private readonly IAppDataService _dataService;
    private readonly DatabaseHelper _databaseHelper;

    public UserRepositoryTests(DatabaseFixture databaseFixture)
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
    public async Task ExistsByEmail_NoDataExist_ReturnsFalse()
    {
        //Arrange
        string email = "me@mysql.com";

        //Act
        bool exists = await _dataService.Users.ExistsByEmail(email);

        //Assert
        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task ExistsByEmail_DataExists_ReturnsTrue()
    {
        //Arrange
        User user = await CreateRandomUser();

        //Act
        bool exists = await _dataService.Users.ExistsByEmail(user.Email!);

        //Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task GetByEmail_NoDataExist_ReturnsNull()
    {
        //Arrange
        string email = "me@mysql.com";

        //Act
        User? user = await _dataService.Users.GetByEmail(email);

        //Assert
        user.ShouldBeNull();
    }

    [Fact]
    public async Task GetByEmail_DataExists_ReturnsTrue()
    {
        //Arrange
        User user = await CreateRandomUser();

        //Act
        User? got = await _dataService.Users.GetByEmail(user.Email!);

        //Assert
        got.ShouldNotBeNull();
        got.Id.ShouldBe(user.Id);
    }

    private async Task<User> CreateRandomUser()
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

        return user;
    }
}