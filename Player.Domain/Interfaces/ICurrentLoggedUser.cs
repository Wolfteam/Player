using Player.Domain.Enums;

namespace Player.Domain.Interfaces;

public interface ICurrentLoggedUser
{
    public long Id { get; }
    public string Email { get; }
    public PermissionType PlaylistPermissions { get; }
    public PermissionType MediaPermissions { get; }
}