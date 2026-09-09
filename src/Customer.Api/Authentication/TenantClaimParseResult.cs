using Customer.Common.Identity;

namespace Customer.Api.Authentication;

public sealed record TenantClaimParseResult(
    TenantResolutionStatus Status,
    string? TenantId,
    string? Error)
{
    public static TenantClaimParseResult Missing() =>
        new(TenantResolutionStatus.Missing, null, "The access token does not contain a tenant_id claim.");

    public static TenantClaimParseResult Malformed(string error) =>
        new(TenantResolutionStatus.Malformed, null, error);

    public static TenantClaimParseResult Valid(string tenantId) =>
        new(TenantResolutionStatus.Valid, tenantId, null);
}
