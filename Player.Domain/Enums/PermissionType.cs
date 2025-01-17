namespace Player.Domain.Enums;

[Flags]
public enum PermissionType
{
    None = 0,
    Read = 1 << 0, //1
    Create = 1 << 1, //2
    Delete = 1 << 2, //4
    Update = 1 << 3, //8,
    All = Read | Create | Delete | Update
}