using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MySql;

namespace Player.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _databaseContainer;
    public DatabaseHelper DatabaseHelper { get; private set; } = null!;

    public CustomWebApplicationFactory()
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
        DatabaseHelper = new DatabaseHelper(connectionString);
        await DatabaseHelper.InitializeAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await DatabaseHelper.DisposeAsync();
        await _databaseContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _databaseContainer.GetConnectionString());
        builder.UseEnvironment("Development");
    }
}