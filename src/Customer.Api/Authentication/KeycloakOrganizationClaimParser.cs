using System.Security.Claims;
using System.Text.Json;
using Customer.Common.Identity;

namespace Customer.Api.Authentication;

public sealed class KeycloakOrganizationClaimParser
{
    public OrganizationClaimParseResult Parse(IEnumerable<Claim> claims)
    {
        var organizationClaims = claims
            .Where(claim => claim.Type == KeycloakClaimTypes.Organization)
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();

        if (organizationClaims.Count == 0)
        {
            return OrganizationClaimParseResult.Missing();
        }

        try
        {
            if (organizationClaims.Count == 1 && LooksLikeJsonObject(organizationClaims[0]))
            {
                return ParseOrganizationObject(organizationClaims[0]);
            }

            return OrganizationClaimParseResult.Ambiguous();
        }
        catch (JsonException)
        {
            return OrganizationClaimParseResult.Malformed("The organization claim is not valid JSON.");
        }
    }

    private static OrganizationClaimParseResult ParseOrganizationObject(string rawJson)
    {
        using var document = JsonDocument.Parse(rawJson);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            return OrganizationClaimParseResult.Malformed("The organization claim must be a JSON object.");
        }

        var properties = document.RootElement.EnumerateObject().ToList();
        if (properties.Count == 0)
        {
            return OrganizationClaimParseResult.Missing();
        }

        if (properties.Count > 1)
        {
            return OrganizationClaimParseResult.Ambiguous();
        }

        var organization = properties[0];
        if (organization.Value.ValueKind != JsonValueKind.Object ||
            !organization.Value.TryGetProperty("id", out var idElement) ||
            idElement.ValueKind != JsonValueKind.String)
        {
            return OrganizationClaimParseResult.Malformed("The organization claim must include a single organization id.");
        }

        var tenantId = idElement.GetString();
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return OrganizationClaimParseResult.Malformed("The organization id is empty.");
        }

        if (string.IsNullOrWhiteSpace(organization.Name))
        {
            return OrganizationClaimParseResult.Malformed("The organization alias is empty.");
        }

        return OrganizationClaimParseResult.Valid(tenantId, organization.Name);
    }

    private static bool LooksLikeJsonObject(string value) =>
        value.TrimStart().StartsWith('{');
}
