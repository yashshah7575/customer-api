using Customer.Common.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Customer.Api.Authorization;

public sealed class TenantContextAuthorizationHandler : AuthorizationHandler<TenantContextRequirement>
{
    private readonly TenantContext _tenantContext;

    public TenantContextAuthorizationHandler(TenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantContextRequirement requirement)
    {
        if (_tenantContext.CanAccessTenantData)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
