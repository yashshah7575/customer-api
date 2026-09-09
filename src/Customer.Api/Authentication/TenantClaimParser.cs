using System.Security.Claims;
using Customer.Common.Identity;

namespace Customer.Api.Authentication;

public sealed class TenantClaimParser
{
    public TenantClaimParseResult Parse(IEnumerable<Claim> claims)
    {
        var tenantClaims = claims
            .Where(claim => claim.Type == KeycloakClaimTypes.TenantId)
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (tenantClaims.Count == 0)
        {
            return TenantClaimParseResult.Missing();
        }

        if (tenantClaims.Count > 1)
        {
            return TenantClaimParseResult.Malformed("The access token contains more than one tenant_id claim.");
        }

        return TenantClaimParseResult.Valid(tenantClaims[0]);
    }
}
