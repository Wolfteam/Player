using System.Security.Claims;
using Player.Domain.Enums;
using Player.Domain.Extensions;
using Player.Domain.Interfaces;

namespace Player.API.Models;

public class CurrentLoggedUser : ICurrentLoggedUser
{
    public long Id { get; }
    public string Email { get; }
    public PermissionType PlaylistPermissions { get; }
    public PermissionType MediaPermissions { get; }

    public CurrentLoggedUser(IHttpContextAccessor context)
    {
        var httpContext = context.HttpContext;

        Id = long.Parse(GetClaimValue(httpContext, ClaimTypes.NameIdentifier)
                        ?? throw new InvalidOperationException("User id was not found"));
        Email = GetClaimValueOrNa(httpContext, ClaimTypes.Email)
                ?? throw new InvalidOperationException("User email was not found");
        if (Enum.TryParse(GetClaimValue(httpContext, AppPermissions.PlaylistPermissionsClaim), out PermissionType pl))
        {
            PlaylistPermissions = pl;
        }

        if (Enum.TryParse(GetClaimValue(httpContext, AppPermissions.MediaPermissionsClaim), out PermissionType mp))
        {
            MediaPermissions = mp;
        }
    }

    private static string? GetClaimValueOrNa(HttpContext? context, params string[] keys)
    {
        return GetClaimValue(context, keys);
    }

    private static string? GetClaimValue(HttpContext? context, params string[] keys)
    {
        return keys
            .Select(key => context?.User.Claims.FirstOrDefault(c => c.Type == key)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }
}