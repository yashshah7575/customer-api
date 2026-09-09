using System.Security.Claims;
using System.Text.Json;
using Customer.Common.Authorization;

namespace Customer.Api.Authentication;

public sealed class KeycloakRoleNormalizer
{
    public const string ResourceClientId = "customer-api";

    public IReadOnlyCollection<string> Normalize(IEnumerable<Claim> claims)
    {
        var roles = new HashSet<string>(StringComparer.Ordinal);

        foreach (var claim in claims.Where(c => c.Type == KeycloakClaimTypes.Roles))
        {
            AddKnownRole(roles, claim.Value);
        }

        foreach (var claim in claims.Where(c => c.Type == KeycloakClaimTypes.ResourceAccess))
        {
            AddRolesFromResourceAccess(roles, claim.Value);
        }

        foreach (var claim in claims.Where(c => c.Type == KeycloakClaimTypes.RealmAccess))
        {
            AddRolesFromRoleContainer(roles, claim.Value);
        }

        return roles;
    }

    private static void AddRolesFromResourceAccess(HashSet<string> roles, string rawJson)
    {
        if (!LooksLikeJsonObject(rawJson))
        {
            return;
        }

        using var document = JsonDocument.Parse(rawJson);
        if (!document.RootElement.TryGetProperty(ResourceClientId, out var client) ||
            client.ValueKind != JsonValueKind.Object ||
            !client.TryGetProperty("roles", out var roleArray) ||
            roleArray.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var role in roleArray.EnumerateArray())
        {
            if (role.ValueKind == JsonValueKind.String)
            {
                AddKnownRole(roles, role.GetString());
            }
        }
    }

    private static void AddRolesFromRoleContainer(HashSet<string> roles, string rawJson)
    {
        if (!LooksLikeJsonObject(rawJson))
        {
            return;
        }

        using var document = JsonDocument.Parse(rawJson);
        if (!document.RootElement.TryGetProperty("roles", out var roleArray) ||
            roleArray.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var role in roleArray.EnumerateArray())
        {
            if (role.ValueKind == JsonValueKind.String)
            {
                AddKnownRole(roles, role.GetString());
            }
        }
    }

    private static void AddKnownRole(HashSet<string> roles, string? role)
    {
        if (role is ApplicationRoles.PlatformAdmin or
            ApplicationRoles.TenantAdmin or
            ApplicationRoles.TenantEditor or
            ApplicationRoles.TenantReader)
        {
            roles.Add(role);
        }
    }

    private static bool LooksLikeJsonObject(string value) =>
        !string.IsNullOrWhiteSpace(value) && value.TrimStart().StartsWith('{');
}
