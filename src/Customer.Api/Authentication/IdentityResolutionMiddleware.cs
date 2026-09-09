using System.Security.Claims;
using Customer.Common.Authorization;
using Customer.Common.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Authentication;

public sealed class IdentityResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdentityResolutionMiddleware> _logger;

    public IdentityResolutionMiddleware(RequestDelegate next, ILogger<IdentityResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        TenantContext tenantContext,
        ApplicationIdentity applicationIdentity,
        TenantClaimParser tenantParser,
        KeycloakRoleNormalizer roleNormalizer)
    {
        var principal = context.User;
        if (principal.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        tenantContext.IsAuthenticated = true;
        tenantContext.SubjectId = FirstClaim(principal, KeycloakClaimTypes.Subject);
        tenantContext.Username = FirstClaim(principal, KeycloakClaimTypes.PreferredUsername);

        applicationIdentity.SetRoles(roleNormalizer.Normalize(principal.Claims));
        tenantContext.IsPlatformAdmin = applicationIdentity.HasPermission(ApplicationPermissions.PlatformAdminister);

        var tenant = tenantParser.Parse(principal.Claims);
        tenantContext.TenantStatus = tenant.Status;
        tenantContext.TenantId = tenant.TenantId;

        if (RequiresTenantContext(context) && !tenantContext.CanAccessTenantData)
        {
            _logger.LogWarning(
                "Rejected tenant-scoped request for subject {SubjectId} because tenant context was {TenantStatus}",
                tenantContext.SubjectId,
                tenant.Status);
            await WriteTenantRejectionAsync(context, tenant);
            return;
        }

        await _next(context);
    }

    private static bool RequiresTenantContext(HttpContext context) =>
        context.GetEndpoint()?.Metadata.GetMetadata<RequireTenantAttribute>() is not null;

    private static async Task WriteTenantRejectionAsync(HttpContext context, TenantClaimParseResult tenant)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Tenant context required",
            Detail = tenant.Error ?? "Tenant-scoped requests require a tenant_id claim unless the caller is PlatformAdmin.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string? FirstClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value;
}
