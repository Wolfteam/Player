using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Player.Domain.Extensions;

namespace Player.API.Authorization;

public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        string[] split = policyName.Split("_");

        if (split.Length != 2)
        {
            throw new ArgumentOutOfRangeException("The provided policy name is not valid");
        }

        string category = split.First();
        string enumValue = split.Last();

        string claimType = category switch
        {
            AppPermissions.MediaCategory => AppPermissions.MediaPermissionsClaim,
            AppPermissions.PlaylistCategory => AppPermissions.PlaylistPermissionsClaim,
            _ => throw new ArgumentOutOfRangeException(nameof(policyName), policyName, "Unsupported policy name")
        };

        return await base.GetPolicyAsync(policyName)
               ?? new AuthorizationPolicyBuilder()
                   .RequireClaim(claimType)
                   .AddRequirements(new PermissionRequirement(claimType, enumValue))
                   .Build();
    }
}