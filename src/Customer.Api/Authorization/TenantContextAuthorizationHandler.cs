using Customer.Common.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Customer.Api.Authorization;

public sealed class TenantContextAuthorizationHandler : AuthorizationHandler<TenantContextRequirement>
{
    private readonly ITenantContext _tenantContext;

    public TenantContextAuthorizationHandler(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantContextRequirement requirement)
    {
        if (_tenantContext.HasValidTenant)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
