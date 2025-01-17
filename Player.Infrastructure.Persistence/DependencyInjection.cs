using FluentMigrator.Runner;
using FreeSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Player.Domain.Entities;
using Player.Domain.Interfaces.Services;
using Player.Domain.Utils;
using Player.Infrastructure.Persistence.Migrations;

namespace Player.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        Check.NotEmpty(connectionString, nameof(connectionString));

        services.AddScoped<IFreeSql>(_ =>
        {
            IFreeSql context = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, connectionString)
#if DEBUG
                .UseMonitorCommand(
                    cmd => Console.WriteLine($"ExecutingSql：{cmd.CommandText}"),
                    (_, sql) => Console.WriteLine($"ExecutedSql：{sql}"))
#endif
                .UseAutoSyncStructure(false)
                .UseExitAutoDisposePool(false)
                .Build();

            return context;
        });

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddMySql8()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(Init).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        services.AddScoped<IAppDataService, AppDataService>();
        services.AddScoped<IUserStore<User>, CustomUserStore>();
        services.AddScoped<IUserClaimStore<User>, CustomUserStore>();
        services.AddScoped<IUserPasswordStore<User>, CustomUserStore>();
        services.AddScoped<IUserSecurityStampStore<User>, CustomUserStore>();
        services.AddScoped<IUserLockoutStore<User>, CustomUserStore>();
        services.AddScoped<IUserEmailStore<User>, CustomUserStore>();
        return services;
    }

    public static void ApplyDatabaseMigrations(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}