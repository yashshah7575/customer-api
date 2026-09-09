using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Customer.Common.Authorization;
using Customer.Common.Tenancy;
using Microsoft.IdentityModel.Tokens;

namespace Customer.Api.Integration.Tests;

public static class TestJwtIssuer
{
    public const string Issuer = "https://issuer.test/realms/customer-api-demo";
    public const string Audience = "customer-api";

    public static readonly SymmetricSecurityKey SigningKey = new(
        Encoding.UTF8.GetBytes("local-test-signing-key-256-bit-min!"));

    public static string CreateToken(
        string subject,
        string username,
        string? tenantId,
        IEnumerable<string> roles,
        string? issuer = null,
        string? audience = null,
        DateTime? expires = null,
        SecurityKey? signingKey = null)
    {
        var claims = new List<Claim>
        {
            new("sub", subject),
            new("preferred_username", username)
        };

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            claims.Add(new Claim("tenant_id", tenantId));
        }

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
        }

        var tokenExpires = expires ?? DateTime.UtcNow.AddMinutes(30);
        var token = new JwtSecurityToken(
            issuer: issuer ?? Issuer,
            audience: audience ?? Audience,
            claims: claims,
            notBefore: tokenExpires.AddMinutes(-30),
            expires: tokenExpires,
            signingCredentials: new SigningCredentials(
                signingKey ?? SigningKey,
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string AliceAdmin() =>
        CreateToken("alice-admin", "alice-admin", DemoTenants.CustomerA, [ApplicationRoles.CustomerAdmin]);

    public static string AliceOperator() =>
        CreateToken("alice-operator", "alice-operator", DemoTenants.CustomerA, [ApplicationRoles.Operator]);

    public static string BobAdmin() =>
        CreateToken("bob-admin", "bob-admin", DemoTenants.CustomerB, [ApplicationRoles.CustomerAdmin]);

    public static string BobViewer() =>
        CreateToken("bob-viewer", "bob-viewer", DemoTenants.CustomerB, [ApplicationRoles.Viewer]);

    public static string PlatformAdmin() =>
        CreateToken("platform-admin", "platform-admin", null, [ApplicationRoles.PlatformAdmin]);

    public static string ServiceClient() =>
        CreateToken("customer-a-integration", "service-account-customer-a-integration", DemoTenants.CustomerA, [ApplicationRoles.ServiceClient]);
}
