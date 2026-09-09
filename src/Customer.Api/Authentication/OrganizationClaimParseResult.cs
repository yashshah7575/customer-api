using Customer.Common.Identity;

namespace Customer.Api.Authentication;

public sealed record OrganizationClaimParseResult(
    TenantResolutionStatus Status,
    string? TenantId,
    string? TenantAlias,
    string? Error)
{
    public static OrganizationClaimParseResult Missing() =>
        new(TenantResolutionStatus.Missing, null, null, "The access token does not contain an organization claim.");

    public static OrganizationClaimParseResult Ambiguous() =>
        new(TenantResolutionStatus.Ambiguous, null, null, "The access token contains more than one organization. Tenant-scoped requests require exactly one.");

    public static OrganizationClaimParseResult Malformed(string error) =>
        new(TenantResolutionStatus.Malformed, null, null, error);

    public static OrganizationClaimParseResult Valid(string tenantId, string tenantAlias) =>
        new(TenantResolutionStatus.Valid, tenantId, tenantAlias, null);
}
