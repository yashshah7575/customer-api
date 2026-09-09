namespace Customer.Common.Identity;

/// <summary>
/// Request-scoped identity derived from a validated access token.
/// Never populated from headers, routes, query strings, or request bodies.
/// </summary>
public interface ITenantContext
{
    string? TenantId { get; }

    string? SubjectId { get; }

    string? Username { get; }

    bool IsAuthenticated { get; }

    bool IsPlatformAdmin { get; }

    TenantResolutionStatus TenantStatus { get; }

    bool HasValidTenant { get; }

    /// <summary>
    /// Platform administrators may operate without a tenant_id. Everyone else needs a valid one.
    /// </summary>
    bool CanAccessTenantData { get; }
}
