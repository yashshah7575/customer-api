using System.Security.Claims;
using System.Text.Json;
using Customer.Common.Authorization;

namespace Customer.Api.Authentication;

public sealed class KeycloakRoleNormalizer
{
    public const string ResourceClientId = "customer-api";

    private static readonly HashSet<string> KnownRoles =
    [
        ApplicationRoles.PlatformAdmin,
        ApplicationRoles.CustomerAdmin,
        ApplicationRoles.Operator,
        ApplicationRoles.Viewer,
        ApplicationRoles.ServiceClient
    ];

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

        return roles;
    }

    private static void AddRolesFromResourceAccess(HashSet<string> roles, string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson) || !rawJson.TrimStart().StartsWith('{'))
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

    private static void AddKnownRole(HashSet<string> roles, string? role)
    {
        if (role is not null && KnownRoles.Contains(role))
        {
            roles.Add(role);
        }
    }
}
