using System.Security.Claims;
using Customer.Api.Authentication;
using Customer.Common.Identity;
using Customer.Common.Tenancy;
using FluentAssertions;

namespace Customer.Api.Tests.Authentication;

public class KeycloakOrganizationClaimParserTests
{
    private readonly KeycloakOrganizationClaimParser _parser = new();

    [Fact]
    public void Parse_MissingOrganization_ReturnsMissing()
    {
        var result = _parser.Parse([new Claim("sub", "user-1")]);

        result.Status.Should().Be(TenantResolutionStatus.Missing);
        result.TenantId.Should().BeNull();
    }

    [Fact]
    public void Parse_SingleOrganization_ReturnsValidTenant()
    {
        var organization = "{\"" + DemoTenants.AcmeBankAlias + "\":{\"id\":\"" + DemoTenants.AcmeBankId + "\"}}";

        var result = _parser.Parse([new Claim("organization", organization)]);

        result.Status.Should().Be(TenantResolutionStatus.Valid);
        result.TenantId.Should().Be(DemoTenants.AcmeBankId);
        result.TenantAlias.Should().Be(DemoTenants.AcmeBankAlias);
    }

    [Fact]
    public void Parse_MultipleOrganizations_ReturnsAmbiguous()
    {
        var organization = "{\"" + DemoTenants.AcmeBankAlias + "\":{\"id\":\"" + DemoTenants.AcmeBankId + "\"},\"" + DemoTenants.ContosoFinanceAlias + "\":{\"id\":\"" + DemoTenants.ContosoFinanceId + "\"}}";

        var result = _parser.Parse([new Claim("organization", organization)]);

        result.Status.Should().Be(TenantResolutionStatus.Ambiguous);
        result.TenantId.Should().BeNull();
    }

    [Fact]
    public void Parse_OrganizationWithoutId_ReturnsMalformed()
    {
        var result = _parser.Parse([new Claim("organization", """{"acme-bank":{}}""")]);

        result.Status.Should().Be(TenantResolutionStatus.Malformed);
    }
}
