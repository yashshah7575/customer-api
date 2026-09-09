namespace Customer.Common.Identity;

public sealed class TenantContext : ITenantContext
{
    public string? TenantId { get; set; }

    public string? SubjectId { get; set; }

    public string? Username { get; set; }

    public bool IsAuthenticated { get; set; }

    public bool IsPlatformAdmin { get; set; }

    public TenantResolutionStatus TenantStatus { get; set; } = TenantResolutionStatus.None;

    public bool HasValidTenant =>
        TenantStatus == TenantResolutionStatus.Valid &&
        !string.IsNullOrWhiteSpace(TenantId);

    public bool CanAccessTenantData => IsPlatformAdmin || HasValidTenant;
}
