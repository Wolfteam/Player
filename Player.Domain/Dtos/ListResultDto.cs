using Player.Domain.Enums;
using Player.Domain.Extensions;

namespace Player.Domain.Dtos;

public class ListResultDto<T> : ResultDto<List<T>>
{
    public int Records
        => Result!.Count;

    public ListResultDto() : this(Enumerable.Empty<T>().ToList())
    {
    }

    public ListResultDto(List<T> results) : base(results)
    {

    }

    public ListResultDto(AppMessageType type, string? details) : base(type, details)
    {
        Result = Enumerable.Empty<T>().ToList();
    }
}

public static class ListResult
{
    public static ListResultDto<T> Success<T>(List<T> result, AppMessageType? type = null)
    {
        return new ListResultDto<T>(result)
        {
            Message = type?.GetErrorMsg(),
            MessageId = type?.GetErrorCode(),
            MessageType = type
        };
    }

    public static ListResultDto<T> Empty<T>()
        => new ListResultDto<T>();

    public static ListResultDto<T> InvalidRequest<T>(string details)
        => new ListResultDto<T>(AppMessageType.InvalidRequest, details);

    public static ListResultDto<T> InvalidRequest<T>(AppMessageType type, string details)
        => new ListResultDto<T>(type, details);

    public static ListResultDto<T> ResourceAlreadyExist<T>(string details)
        => new ListResultDto<T>(AppMessageType.ResourceAlreadyExists, details);

    public static ListResultDto<T> NotFound<T>(string details)
        => new ListResultDto<T>(AppMessageType.NotFound, details);

    public static ListResultDto<T> UnknownError<T>(string details)
        => new ListResultDto<T>(AppMessageType.UnknownError, details);

    public static ListResultDto<T> FromOther<T>(EmptyResultDto result)
        => new ListResultDto<T>(result.MessageType!.Value, null)
        {
            Message = result.Message,
            MessageId = result.MessageId,
        };

    public static ListResultDto<T> InvalidId<T>(long id)
        => InvalidRequest<T>($"The provided id = {id} is not valid");
}