using System.Security.Claims;
using Customer.Api.Authentication;
using Customer.Common.Authorization;
using FluentAssertions;

namespace Customer.Api.Tests.Authentication;

public class KeycloakRoleNormalizerTests
{
    private readonly KeycloakRoleNormalizer _normalizer = new();

    [Fact]
    public void Normalize_ReadsMappedRolesClaim()
    {
        var roles = _normalizer.Normalize([new Claim("roles", ApplicationRoles.CustomerAdmin)]);

        roles.Should().BeEquivalentTo(ApplicationRoles.CustomerAdmin);
    }

    [Fact]
    public void Normalize_ReadsClientRolesFromResourceAccess()
    {
        var resourceAccess = """{"customer-api":{"roles":["Viewer","Operator"]}}""";

        var roles = _normalizer.Normalize([new Claim("resource_access", resourceAccess)]);

        roles.Should().BeEquivalentTo(ApplicationRoles.Viewer, ApplicationRoles.Operator);
    }

    [Fact]
    public void Normalize_IgnoresUnknownRoles()
    {
        var roles = _normalizer.Normalize(
        [
            new Claim("roles", "realm-admin"),
            new Claim("roles", ApplicationRoles.Viewer)
        ]);

        roles.Should().BeEquivalentTo(ApplicationRoles.Viewer);
    }
}
