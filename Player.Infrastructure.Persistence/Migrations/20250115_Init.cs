using FluentMigrator;
using Player.Domain.Entities;

namespace Player.Infrastructure.Persistence.Migrations;

[Migration(1)]
public class Init : Migration
{
    public override void Up()
    {
        Create.Table(nameof(User))
            .WithColumn(nameof(User.Id)).AsInt64().PrimaryKey().Identity()
            .WithColumn(nameof(User.UserName)).AsString(100).NotNullable().Unique()
            .WithColumn(nameof(User.NormalizedUserName)).AsString(100).NotNullable().Unique()
            .WithColumn(nameof(User.Email)).AsString(100).NotNullable().Unique()
            .WithColumn(nameof(User.NormalizedEmail)).AsString(100).NotNullable().Unique()
            .WithColumn(nameof(User.EmailConfirmed)).AsBoolean().NotNullable()
            .WithColumn(nameof(User.PasswordHash)).AsString().Nullable()
            .WithColumn(nameof(User.SecurityStamp)).AsString().Nullable()
            .WithColumn(nameof(User.ConcurrencyStamp)).AsString().Nullable()
            .WithColumn(nameof(User.PhoneNumber)).AsString(30).Nullable()
            .WithColumn(nameof(User.PhoneNumberConfirmed)).AsBoolean().NotNullable()
            .WithColumn(nameof(User.TwoFactorEnabled)).AsBoolean().NotNullable()
            .WithColumn(nameof(User.LockoutEnd)).AsDateTime().Nullable()
            .WithColumn(nameof(User.LockoutEnabled)).AsBoolean().NotNullable()
            .WithColumn(nameof(User.AccessFailedCount)).AsInt32().NotNullable()
            .WithColumn(nameof(User.FirstName)).AsString(100).NotNullable()
            .WithColumn(nameof(User.LastName)).AsString(100).NotNullable()
            .WithColumn(nameof(User.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(User.UpdatedAt)).AsDateTime().Nullable();

        Create.Table(nameof(Playlist))
            .WithColumn(nameof(Playlist.Id)).AsInt64().PrimaryKey().Identity()
            .WithColumn(nameof(Playlist.Name)).AsString(100).NotNullable()
            .WithColumn(nameof(Playlist.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(Playlist.UpdatedAt)).AsDateTime().Nullable();

        Create.Table(nameof(Media))
            .WithColumn(nameof(Media.Id)).AsInt64().PrimaryKey().Identity()
            .WithColumn(nameof(Media.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(Media.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(Media.UpdatedAt)).AsDateTime().Nullable()
            .WithColumn(nameof(Media.PlaylistId)).AsInt64().NotNullable().ForeignKey(nameof(Playlist), nameof(Playlist.Id));
    }

    public override void Down()
    {
        Delete.Table(nameof(User));
        Delete.Table(nameof(Playlist));
        Delete.Table(nameof(Media));
    }
}