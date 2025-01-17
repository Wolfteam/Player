namespace Player.Domain.Enums;

public enum AppMessageType
{
    UnknownError = 1,
    InvalidRequest = 2,
    NotFound = 3,
    ResourceAlreadyExists = 4,
    UserIsLockedOut = 5
}