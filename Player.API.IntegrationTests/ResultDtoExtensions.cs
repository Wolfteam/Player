using Player.Domain.Dtos;
using Player.Domain.Enums;

namespace Player.API.IntegrationTests;

public static class ResultDtoExtensions
{
    public static void EmptyResultShouldBeSucceed(this EmptyResultDto response, AppMessageType? type = null)
        => AssertEmptyResult(response, true, type);

    public static void EmptyResultShouldBeInvalidRequest(this EmptyResultDto response, AppMessageType? appMessageType = null)
        => AssertEmptyResult(response, false, appMessageType ?? AppMessageType.InvalidRequest);

    public static void EmptyResultShouldBeNotFound(this EmptyResultDto response, AppMessageType? appMessageType = null)
        => AssertEmptyResult(response, false, appMessageType ?? AppMessageType.NotFound);

    public static void EmptyResultShouldBeUnknownError(this EmptyResultDto response)
        => AssertEmptyResult(response, false, AppMessageType.UnknownError);

    public static void EmptyResultShouldFail(this EmptyResultDto response, AppMessageType type)
        => AssertEmptyResult(response, false, type);

    public static void ResultShouldBeSucceed<T>(this ResultDto<T> response, AppMessageType? type = null) where T : class
        => AssertResult(response, true, type);

    public static void ResultShouldBeInvalidRequest<T>(this ResultDto<T> response, AppMessageType? appMessageType = null) where T : class
        => AssertResult(response, false, appMessageType ?? AppMessageType.InvalidRequest);

    public static void ResultShouldBeNotFound<T>(this ResultDto<T> response) where T : class
        => AssertResult(response, false, AppMessageType.NotFound);

    public static void ResultShouldBeUnknownError<T>(this ResultDto<T> response) where T : class
        => AssertResult(response, false, AppMessageType.UnknownError);

    public static void ResultShouldFail<T>(this ResultDto<T> response, AppMessageType type) where T : class
        => AssertResult(response, false, type);

    public static void ListResultShouldBeSucceed<T>(this ListResultDto<T> response, AppMessageType? type = null) where T : class
        => AssertListResult(response, true, type);

    public static void ListResultShouldBeInvalidRequest<T>(this ListResultDto<T> response) where T : class
        => AssertListResult(response, false, AppMessageType.InvalidRequest);

    public static void ListResultShouldBeNotFound<T>(this ListResultDto<T> response) where T : class
        => AssertListResult(response, false, AppMessageType.NotFound);

    public static void ListResultShouldBeUnknownError<T>(this ListResultDto<T> response) where T : class
        => AssertListResult(response, false, AppMessageType.UnknownError);

    public static void ListResultShouldFail<T>(this ListResultDto<T> response, AppMessageType type) where T : class
        => AssertListResult(response, false, type);

    public static void ListResultShouldBeEmpty<T>(this ListResultDto<T> response, AppMessageType? type = null)
        where T : class
    {
        AssertEmptyResult(response, true, type);
        response.Result.ShouldNotBeNull();
        response.Result.ShouldBeEmpty();
    }

    private static void AssertEmptyResult(EmptyResultDto response, bool shouldSucceed, AppMessageType? type)
    {
        response.ShouldNotBeNull();
        if (shouldSucceed)
        {
            response.Succeed.ShouldBeTrue($"Response did not succeed, error = {response.MessageId} - {response.Message}");
        }
        else
        {
            response.Succeed.ShouldBeFalse();
            response.Message.ShouldNotBeNullOrWhiteSpace();
            response.Message.ShouldNotBeNullOrWhiteSpace();
        }

        if (type != null)
        {
            response.MessageType.ShouldBe(type, $"Response message = {response.Message} - {response.MessageId}");
        }
    }

    private static void AssertResult<T>(ResultDto<T> response, bool shouldSucceed, AppMessageType? type)
        where T : class
    {
        AssertEmptyResult(response, shouldSucceed, type);
        if (shouldSucceed)
        {
            response.Result.ShouldNotBeNull();
        }
    }

    private static void AssertListResult<T>(ListResultDto<T> response, bool shouldSucceed, AppMessageType? type)
        where T : class
    {
        AssertEmptyResult(response, shouldSucceed, type);
        if (shouldSucceed)
        {
            response.Result.ShouldNotBeEmpty();
        }
    }
}