using Player.Domain.Enums;
using Player.Domain.Interfaces;

namespace Player.Application.UnitTests;

public class TestCurrentUser : ICurrentLoggedUser
{
    public long Id
        => 666;

    public string Email
        => "nahida@saikoudesu.com";

    public PermissionType PlaylistPermissions
        => PermissionType.All;

    public PermissionType MediaPermissions
        => PermissionType.All;
}