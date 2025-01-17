using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Player.Domain.Extensions;

namespace Player.API.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        List<Claim> permissions = context.User.Claims.Where(c => c.Type == requirement.ClaimType).ToList();
        foreach (Claim permission in permissions)
        {
            if (permission.Value.IsThisPermissionAllowed(requirement.ClaimType, requirement.ClaimValue))
                context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
