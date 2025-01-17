using Player.Domain.Enums;

namespace Player.Domain.Extensions;

public static class AppPermissions
{
    public const string PlaylistCategory = "playlist";
    public const string MediaCategory = "medias";

    public const string PlaylistPermissionsClaim = "permissions." + PlaylistCategory;
    public const string MediaPermissionsClaim = "permissions." + MediaCategory;

    public static readonly string[] SupportedClaims = [PlaylistPermissionsClaim, MediaPermissionsClaim];
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
            throw new ArgumentOutOfRangeException(
                nameof(permission),
                permission,
                "The current permission couldn't be converted to a valid long value.");
        }

        if (!long.TryParse(permissionName, out long requiredPermission))
        {
            throw new ArgumentOutOfRangeException(
                nameof(permissionName),
                permissionName,
                "The required permission couldn't be converted to a valid integer value.");
        }

        if (!AppPermissions.SupportedClaims.Contains(claimType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(claimType),
                claimType,
                "The provided claim type is not supported");
        }

        bool exists = Enum.GetValues<PermissionType>()
            .Select(val => (long)Convert.ChangeType(val, typeof(long)))
            .Any(enumValue =>
                enumValue != 0 &&
                (enumValue & requiredPermission) == enumValue &&
                (requiredPermission & currentPermission) == requiredPermission);

        return exists;
    }
}