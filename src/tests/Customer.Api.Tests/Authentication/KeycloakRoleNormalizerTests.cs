using System.Security.Claims;
using Customer.Api.Authentication;
using Customer.Common.Authorization;
using FluentAssertions;

namespace Customer.Api.Tests.Authentication;

public class KeycloakRoleNormalizerTests
{
    private readonly KeycloakRoleNormalizer _normalizer = new();

    [Fact]
    public void Normalize_ReadsClientRolesFromResourceAccess()
    {
        var resourceAccess = """{"customer-api":{"roles":["tenant-reader","tenant-editor"]}}""";

        var roles = _normalizer.Normalize([new Claim("resource_access", resourceAccess)]);

        roles.Should().BeEquivalentTo(ApplicationRoles.TenantReader, ApplicationRoles.TenantEditor);
    }

    [Fact]
    public void Normalize_IgnoresUnknownRoles()
    {
        var resourceAccess = """{"customer-api":{"roles":["realm-admin","tenant-reader"]}}""";

        var roles = _normalizer.Normalize([new Claim("resource_access", resourceAccess)]);

        roles.Should().BeEquivalentTo(ApplicationRoles.TenantReader);
    }

    [Fact]
    public void Normalize_ReadsSimpleRolesClaim()
    {
        var roles = _normalizer.Normalize([new Claim("roles", ApplicationRoles.PlatformAdmin)]);

        roles.Should().BeEquivalentTo(ApplicationRoles.PlatformAdmin);
    }
}
