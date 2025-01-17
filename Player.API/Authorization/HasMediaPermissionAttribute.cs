using Microsoft.AspNetCore.Authorization;
using Player.Domain.Enums;
using Player.Domain.Extensions;

namespace Player.API.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public abstract class BaseHasPermissionAttribute : AuthorizeAttribute
{
    protected BaseHasPermissionAttribute(string category, string permission)
        : base($"{category}_{permission}")
    {
    }
}

public class HasMediaPermissionAttribute : BaseHasPermissionAttribute
{
    public HasMediaPermissionAttribute(PermissionType permission)
        : base(AppPermissions.MediaCategory, ((long)permission).ToString())
    {
    }
}

public class HasPlaylistPermissionAttribute : BaseHasPermissionAttribute
{
    public HasPlaylistPermissionAttribute(PermissionType permission)
        : base(AppPermissions.PlaylistCategory, ((long)permission).ToString())
    {
    }
}