using Player.Domain.Enums;

namespace Player.Domain.Extensions;

public static class AppMessageTypeExtensions
{
    public static string GetErrorMsg(this AppMessageType msg)
    {
        return msg switch
        {
            AppMessageType.InvalidRequest => "Invalid request",
            AppMessageType.UnknownError => "Unknown error occurred",
            AppMessageType.NotFound => "The resource was not found",
            AppMessageType.ResourceAlreadyExists => "Resource already exists",
            AppMessageType.UserIsLockedOut => "User is locked out",
            _ => throw new ArgumentOutOfRangeException(nameof(msg), msg, null)
        };
    }

    public static string GetErrorCode(this AppMessageType msg)
    {
        if (!Enum.IsDefined(typeof(AppMessageType), msg))
        {
            throw new ArgumentOutOfRangeException(nameof(msg), msg, null);
        }

        int msgId = (int)msg;
        return $"BACKEND_{msgId}";
    }
}