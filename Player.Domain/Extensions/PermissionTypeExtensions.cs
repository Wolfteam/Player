using Player.Domain.Enums;

namespace Player.Domain.Extensions;

public static class AppPermissions
{
    public const string PlaylistPermissionsClaim = "playlist.permissions";
    public const string MediaPermissionsClaim = "media.permissions";
}

public static class PermissionTypeExtensions
{
    public static string GetPermissionStringValue<TEnum>(this TEnum permission)
        where TEnum : Enum
    {
        object val = Convert.ChangeType(permission, typeof(long));
        return val.ToString()!;
    }

    public static bool IsThisPermissionAllowed(this string permission, string claimType, string permissionName)
    {
        if (!long.TryParse(permission, out long currentPermission))
        {
            throw new ArgumentOutOfRangeException(nameof(permission), permission, "The current permission couldn't be converted to a valid long value.");
        }

        if (!long.TryParse(permissionName, out long requiredPermission))
        {
            throw new ArgumentOutOfRangeException(nameof(permissionName), permissionName, "The required permission couldn't be converted to a valid integer value.");
        }

        Type enumType = claimType switch
        {
            AppPermissions.PlaylistPermissionsClaim => typeof(PermissionType),
            AppPermissions.MediaPermissionsClaim => typeof(PermissionType),
            _ => throw new ArgumentOutOfRangeException(nameof(claimType), claimType, null)
        };

        bool exists = new List<long>(Enum.GetValues(enumType).Cast<long>())
            .Any(enumValue =>
                enumValue != 0 &&
                (enumValue & requiredPermission) == enumValue &&
                (requiredPermission & currentPermission) == requiredPermission);

        return exists;
    }
}