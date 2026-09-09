using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Customer.Common.Tenancy;
using Microsoft.IdentityModel.Tokens;

namespace Customer.Api.Integration.Tests;

public static class TestJwtIssuer
{
    public const string Issuer = "https://issuer.test/realms/customer-platform";
    public const string Audience = "customer-api";

    public static readonly SymmetricSecurityKey SigningKey = new(
        Encoding.UTF8.GetBytes("local-test-signing-key-256-bit-min!"));

    public static string CreateToken(
        string subject,
        string username,
        string? tenantId,
        string? tenantAlias,
        IEnumerable<string> roles,
        string? issuer = null,
        string? audience = null,
        DateTime? expires = null)
    {
        var claims = new List<Claim>
        {
            new("sub", subject),
            new("preferred_username", username)
        };

        if (!string.IsNullOrWhiteSpace(tenantId) && !string.IsNullOrWhiteSpace(tenantAlias))
        {
            var organization = $"{{\"{tenantAlias}\":{{\"id\":\"{tenantId}\"}}}}";
            claims.Add(new Claim("organization", organization, JsonClaimValueTypes.Json));
        }

        var roleList = roles.ToList();
        if (roleList.Count > 0)
        {
            var quotedRoles = string.Join(',', roleList.Select(role => $"\"{role}\""));
            var resourceAccess = $"{{\"customer-api\":{{\"roles\":[{quotedRoles}]}}}}";
            claims.Add(new Claim("resource_access", resourceAccess, JsonClaimValueTypes.Json));
        }

        var tokenExpires = expires ?? DateTime.UtcNow.AddMinutes(30);
        var token = new JwtSecurityToken(
            issuer: issuer ?? Issuer,
            audience: audience ?? Audience,
            claims: claims,
            notBefore: tokenExpires.AddMinutes(-30),
            expires: tokenExpires,
            signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string AcmeReader() =>
        CreateToken("acme-reader", "acme.reader", DemoTenants.AcmeBankId, DemoTenants.AcmeBankAlias, ["tenant-reader"]);

    public static string AcmeEditor() =>
        CreateToken("acme-editor", "acme.editor", DemoTenants.AcmeBankId, DemoTenants.AcmeBankAlias, ["tenant-editor"]);

    public static string AcmeAdmin() =>
        CreateToken("acme-admin", "acme.admin", DemoTenants.AcmeBankId, DemoTenants.AcmeBankAlias, ["tenant-admin"]);

    public static string ContosoReader() =>
        CreateToken("contoso-reader", "contoso.reader", DemoTenants.ContosoFinanceId, DemoTenants.ContosoFinanceAlias, ["tenant-reader"]);

    public static string ContosoEditor() =>
        CreateToken("contoso-editor", "contoso.editor", DemoTenants.ContosoFinanceId, DemoTenants.ContosoFinanceAlias, ["tenant-editor"]);

    public static string PlatformAdmin() =>
        CreateToken("platform-admin", "platform.admin", null, null, ["platform-admin"]);
}
