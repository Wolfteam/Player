using Player.Domain.Enums;
using Player.Domain.Extensions;
using Shouldly;
using Xunit;

namespace Player.Domain.UnitTests.Extensions;

public class PermissionTypeExtensionsTests
{
    public static readonly IEnumerable<TheoryDataRow<PermissionType>> Types = [..Enum.GetValues<PermissionType>()];

    [Fact]
    public void GetPermissionStringValue_InvalidPermission_ThrowsException()
    {
        const PermissionType type = (PermissionType)999;
        Should.Throw<ArgumentOutOfRangeException>(() => type.GetPermissionStringValue());
    }

    [Theory]
    [MemberData(nameof(Types))]
    public void GetPermissionStringValue_AllTypesHaveValues_ReturnsValidValue(PermissionType type)
    {
        string value = type.GetPermissionStringValue();
        value.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void IsThisPermissionAllowed_ClaimIsNotSupported_ThrowsException()
    {
        string current = $"{(long)(PermissionType.Read | PermissionType.Create)}";
        string requested = $"{(long)PermissionType.Delete}";

        Should.Throw<ArgumentOutOfRangeException>(() => current.IsThisPermissionAllowed("not-supported", requested));
    }

    [Fact]
    public void IsThisPermissionAllowed_PermissionIsNotAllowed_ReturnsFalse()
    {
        string current = $"{(long)(PermissionType.Read | PermissionType.Create)}";
        string requested = $"{(long)PermissionType.Delete}";

        current.IsThisPermissionAllowed(AppPermissions.PlaylistPermissionsClaim, requested)
            .ShouldBeFalse();
    }

    [Fact]
    public void IsThisPermissionAllowed_PermissionIsAllowed_ReturnsTrue()
    {
        string current = $"{(long)(PermissionType.Read | PermissionType.Create)}";
        string requested = $"{(long)PermissionType.Read}";

        current.IsThisPermissionAllowed(AppPermissions.PlaylistPermissionsClaim, requested)
            .ShouldBeTrue();
    }
}