using System.Security.Claims;
using Customer.Common.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Authentication;

public sealed class IdentityResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public IdentityResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        TenantContext tenantContext,
        ApplicationIdentity applicationIdentity,
        KeycloakOrganizationClaimParser organizationParser,
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

        var organization = organizationParser.Parse(principal.Claims);
        tenantContext.TenantStatus = organization.Status;
        tenantContext.TenantId = organization.TenantId;
        tenantContext.TenantAlias = organization.TenantAlias;

        if (RequiresTenantContext(context) && !tenantContext.HasValidTenant)
        {
            await WriteTenantRejectionAsync(context, organization);
            return;
        }

        await _next(context);
    }

    private static bool RequiresTenantContext(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata.GetMetadata<RequireTenantAttribute>() is not null;
    }

    private static async Task WriteTenantRejectionAsync(HttpContext context, OrganizationClaimParseResult organization)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Tenant context required",
            Detail = organization.Error ?? "Tenant-scoped requests require exactly one organization in the access token.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
        };

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string? FirstClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value;
}
