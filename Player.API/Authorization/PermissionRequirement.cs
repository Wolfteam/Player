using Microsoft.AspNetCore.Authorization;

namespace Player.API.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string ClaimType { get; }
    public string ClaimValue { get; }

    public PermissionRequirement(string claimType, string claimValue)
    {
        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}