using System.Security.Claims;
using Customer.Api.Authentication;
using Customer.Common.Identity;
using Customer.Common.Tenancy;
using FluentAssertions;

namespace Customer.Api.Tests.Authentication;

public class TenantClaimParserTests
{
    private readonly TenantClaimParser _parser = new();

    [Fact]
    public void Parse_MissingTenant_ReturnsMissing()
    {
        var result = _parser.Parse([new Claim("sub", "user-1")]);

        result.Status.Should().Be(TenantResolutionStatus.Missing);
        result.TenantId.Should().BeNull();
    }

    [Fact]
    public void Parse_SingleTenant_ReturnsValid()
    {
        var result = _parser.Parse([new Claim("tenant_id", DemoTenants.CustomerA)]);

        result.Status.Should().Be(TenantResolutionStatus.Valid);
        result.TenantId.Should().Be(DemoTenants.CustomerA);
    }

    [Fact]
    public void Parse_ConflictingTenantClaims_ReturnsMalformed()
    {
        var result = _parser.Parse(
        [
            new Claim("tenant_id", DemoTenants.CustomerA),
            new Claim("tenant_id", DemoTenants.CustomerB)
        ]);

        result.Status.Should().Be(TenantResolutionStatus.Malformed);
    }
}
