using Player.Domain.Enums;
using Xunit;
using Player.Domain.Extensions;
using Shouldly;

namespace Player.Domain.UnitTests.Extensions;

public class AppMessageTypeExtensionsTests
{
    public static readonly IEnumerable<TheoryDataRow<AppMessageType>> Types = [..Enum.GetValues<AppMessageType>()];


    [Fact]
    public void GetErrorMsg_TypeDoesNotExist_ThrowsException()
    {
        const AppMessageType type = (AppMessageType)999;
        Should.Throw<ArgumentOutOfRangeException>(() => type.GetErrorMsg());
    }

    [Theory]
    [MemberData(nameof(Types))]
    public void GetErrorMsg_AllTypesHaveErrorMessages_ReturnsValidMessage(AppMessageType type)
    {
        string msg = type.GetErrorMsg();
        msg.ShouldNotBeNullOrWhiteSpace();
    }


    [Fact]
    public void GetErrorCode_TypeDoesNotExist_ThrowsException()
    {
        const AppMessageType type = (AppMessageType)999;
        Should.Throw<ArgumentOutOfRangeException>(() => type.GetErrorCode());
    }

    [Theory]
    [MemberData(nameof(Types))]
    public void GetErrorCode_AllTypesHaveErrorCodes_ReturnsValidCode(AppMessageType type)
    {
        string code = type.GetErrorMsg();
        code.ShouldNotBeNullOrWhiteSpace();
    }
}