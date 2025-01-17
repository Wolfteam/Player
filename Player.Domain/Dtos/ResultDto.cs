using Player.Domain.Enums;
using Player.Domain.Extensions;

namespace Player.Domain.Dtos;

public class ResultDto<T> : EmptyResultDto
{
    public T? Result { get; set; }

    public ResultDto()
    {
    }

    public ResultDto(T? result) : base(true)
    {
        Result = result;
    }

    public ResultDto(AppMessageType type, string? details) : base(type, details)
    {
    }

    public ResultDto(bool succeed, T? result, AppMessageType type) : base(type)
    {
        Succeed = succeed;
        Result = result;
    }
}

public static class Result
{
    public static ResultDto<T> Success<T>(T? result, AppMessageType? type = null)
        => new ResultDto<T>(result)
        {
            Message = type?.GetErrorMsg(),
            MessageId = type?.GetErrorCode(),
            MessageType = type
        };

    public static ResultDto<T> InvalidRequest<T>(string details)
        => new ResultDto<T>(AppMessageType.InvalidRequest, details);

    public static ResultDto<T> InvalidRequest<T>(AppMessageType type, string details)
        => new ResultDto<T>(type, details);

    public static ResultDto<T> ResourceAlreadyExist<T>(string details)
        => new ResultDto<T>(AppMessageType.ResourceAlreadyExists, details);

    public static ResultDto<T> NotFound<T>(string details)
        => new ResultDto<T>(AppMessageType.NotFound, details);

    public static ResultDto<T> UnknownError<T>(string details)
        => new ResultDto<T>(AppMessageType.UnknownError, details);

    public static ResultDto<T> FromOther<T>(EmptyResultDto result)
        => new ResultDto<T>(result.MessageType!.Value, null)
        {
            Message = result.Message,
            MessageId = result.MessageId,
        };

    public static ResultDto<T> FromOther<T>(T result, EmptyResultDto other)
        => new ResultDto<T>(other.MessageType!.Value, null)
        {
            Message = other.Message,
            MessageId = other.MessageId,
            Result = result
        };

    public static ResultDto<T> InvalidId<T>(long id)
        => InvalidRequest<T>($"The provided id = {id} is not valid");

    public static ResultDto<T> Fail<T>(AppMessageType type, string details)
    {
        var result = new ResultDto<T>(type, details);
        result.AppendDetails(details);
        return result;
    }
}