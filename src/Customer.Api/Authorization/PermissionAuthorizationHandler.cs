using Customer.Common.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Customer.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IApplicationIdentity _identity;

    public PermissionAuthorizationHandler(IApplicationIdentity identity)
    {
        _identity = identity;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (_identity.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
