using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Player.Application.Medias;
using Player.Application.Playlists;
using Player.Application.Users;
using Player.Application.Validation;

namespace Player.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidatorService, ValidatorService>();
        services.AddValidatorsFromAssemblies(
            [typeof(DependencyInjection).Assembly],
            ServiceLifetime.Scoped,
            f => !f.ValidatorType.IsNestedPrivate,
            true);

        return services;
    }

    public static IServiceCollection AddUserService(this IServiceCollection services)
    {
        services.TryAddScoped<IUserService, UserService>();
        return services;
    }

    public static IServiceCollection AddPlaylistService(this IServiceCollection services)
    {
        services.TryAddScoped<IPlaylistService, PlaylistService>();
        return services;
    }

    public static IServiceCollection AddMediaService(this IServiceCollection services)
    {
        services.TryAddScoped<IMediasService, MediasService>();
        return services;
    }
}