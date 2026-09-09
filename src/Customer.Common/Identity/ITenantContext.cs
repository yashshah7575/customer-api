namespace Customer.Common.Identity;

/// <summary>
/// Request-scoped tenant and subject identity derived from a validated access token.
/// Never populated from client-supplied headers, routes, or request bodies.
/// </summary>
public interface ITenantContext
{
    string? TenantId { get; }

    string? TenantAlias { get; }

    string? SubjectId { get; }

    string? Username { get; }

    bool IsAuthenticated { get; }

    TenantResolutionStatus TenantStatus { get; }

    bool HasValidTenant { get; }
}
