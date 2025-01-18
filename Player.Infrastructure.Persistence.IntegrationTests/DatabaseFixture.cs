using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Player.Domain.Interfaces.Services;
using Testcontainers.MySql;

namespace Player.Infrastructure.Persistence.IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly MySqlContainer _databaseContainer;

    public IAppDataService DataService { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public DatabaseHelper DatabaseHelper { get; private set; } = null!;

    public DatabaseFixture()
    {
        _databaseContainer = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithDatabase("main")
            .WithUsername("root")
            .WithPassword("1234")
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _databaseContainer.StartAsync();
        string connectionString = _databaseContainer.GetConnectionString();
        ServiceProvider = new HostBuilder().ConfigureServices((_, sp) =>
        {
            sp.AddLogging(c => c.AddDebug().AddConsole()).AddPersistence(connectionString);
        }).Build().Services;
        DataService = ServiceProvider.GetRequiredService<IAppDataService>();
        ServiceProvider.ApplyDatabaseMigrations();
        DatabaseHelper = new DatabaseHelper(connectionString);

        await DatabaseHelper.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DatabaseHelper.DisposeAsync();
        await _databaseContainer.StopAsync();
    }
}