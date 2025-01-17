using Player.Domain.Enums;
using Player.Domain.Extensions;
using Player.Domain.Utils;

namespace Player.Domain.Dtos;

public class EmptyResultDto
{
    public bool Succeed { get; set; }
    public string? Message { get; set; }
    public string? MessageId { get; set; }
    public AppMessageType? MessageType { get; set; }

    public EmptyResultDto()
    {
    }

    public EmptyResultDto(bool succeed)
    {
        Succeed = succeed;
    }

    public EmptyResultDto(AppMessageType msgType)
    {
        Message = msgType.GetErrorMsg();
        MessageId = msgType.GetErrorCode();
        MessageType = msgType;
    }

    public EmptyResultDto(AppMessageType msgType, string? details)
        : this(msgType)
    {
        if (!string.IsNullOrWhiteSpace(details))
            AppendDetails(details);
    }

    public void AppendDetails(string details)
    {
        Check.NotNull(Message, nameof(Message));
        Check.NotEmpty(details, nameof(details));
        Message += $". {Environment.NewLine}{details}";
    }
}

public static class EmptyResult
{
    public static EmptyResultDto Success(AppMessageType? type = null)
        => new EmptyResultDto(true)
        {
            Message = type?.GetErrorMsg(),
            MessageId = type?.GetErrorCode(),
            MessageType = type
        };

    public static EmptyResultDto InvalidRequest(string details)
        => new EmptyResultDto(AppMessageType.InvalidRequest, details);

    public static EmptyResultDto InvalidRequest(AppMessageType type, string details)
        => new EmptyResultDto(type, details);

    public static EmptyResultDto NotFound(string details)
        => new EmptyResultDto(AppMessageType.NotFound, details);

    public static EmptyResultDto UnknownError(string details)
        => new EmptyResultDto(AppMessageType.UnknownError, details);

    public static EmptyResultDto ResourceAlreadyExists(string details)
        => new EmptyResultDto(AppMessageType.ResourceAlreadyExists, details);

    public static EmptyResultDto FromOther(EmptyResultDto result)
        => new EmptyResultDto(result.MessageType!.Value, null)
        {
            Message = result.Message,
            MessageId = result.MessageId,
        };

    public static EmptyResultDto InvalidId(long id)
        => InvalidRequest($"The provided id = {id} is not valid");
}