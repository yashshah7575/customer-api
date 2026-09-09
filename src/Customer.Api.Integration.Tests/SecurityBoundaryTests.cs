using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Customer.Common;
using Customer.Common.Models.Customer;
using Customer.Common.Models.Identity;
using Customer.Common.Tenancy;
using FluentAssertions;

namespace Customer.Api.Integration.Tests;

public class SecurityBoundaryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityBoundaryTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnauthenticatedProtectedRequest_Returns401()
    {
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthenticatedUserWithoutPermission_Returns403()
    {
        Authorize(TestJwtIssuer.PlatformAdmin());

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AcmeReader_CanReadAcmeCustomers()
    {
        Authorize(TestJwtIssuer.AcmeReader());

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().OnlyContain(customer => customer.TenantId == DemoTenants.AcmeBankId);
        result.Data.Should().Contain(customer => customer.Id == DemoTenants.AcmeAliceId);
    }

    [Fact]
    public async Task ContosoReader_CanReadContosoCustomers()
    {
        Authorize(TestJwtIssuer.ContosoReader());

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().OnlyContain(customer => customer.TenantId == DemoTenants.ContosoFinanceId);
        result.Data.Should().Contain(customer => customer.Id == DemoTenants.ContosoCarolId);
    }

    [Fact]
    public async Task AcmeReader_CannotRetrieveContosoCustomerById()
    {
        Authorize(TestJwtIssuer.AcmeReader());

        var response = await _client.GetAsync($"/api/customers/{DemoTenants.ContosoCarolId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TenantReader_CannotCreateCustomer()
    {
        Authorize(TestJwtIssuer.AcmeReader());

        var response = await _client.PostAsJsonAsync("/api/customers", NewCustomer("reader-write@acme.example"));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task TenantReader_CannotUpdateOrDelete()
    {
        Authorize(TestJwtIssuer.AcmeReader());

        var update = await _client.PutAsJsonAsync(
            $"/api/customers/{DemoTenants.AcmeAliceId}",
            new UpdateCustomerRequest { FirstName = "Hacked" });
        update.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var delete = await _client.DeleteAsync($"/api/customers/{DemoTenants.AcmeAliceId}");
        delete.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task TenantEditor_CanWriteWithinOwnTenant()
    {
        Authorize(TestJwtIssuer.AcmeEditor());

        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            NewCustomer($"editor-{Guid.NewGuid():N}@acme.example"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();
        created!.Data.TenantId.Should().Be(DemoTenants.AcmeBankId);
    }

    [Fact]
    public async Task TenantAdmin_CanAccessTenantManagement()
    {
        Authorize(TestJwtIssuer.AcmeAdmin());

        var response = await _client.GetAsync("/api/tenant");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<TenantResponse>>();
        result!.Data.TenantId.Should().Be(DemoTenants.AcmeBankId);
        result.Data.TenantAlias.Should().Be(DemoTenants.AcmeBankAlias);
    }

    [Fact]
    public async Task TenantAdmin_CannotAccessPlatformAdministration()
    {
        Authorize(TestJwtIssuer.AcmeAdmin());

        var response = await _client.GetAsync("/api/platform/tenants");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PlatformAdmin_CanAccessPlatformAdministration()
    {
        Authorize(TestJwtIssuer.PlatformAdmin());

        var response = await _client.GetAsync("/api/platform/tenants");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IReadOnlyCollection<PlatformTenantResponse>>>();
        result!.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task ClientSuppliedTenantId_IsIgnoredOnCreate()
    {
        Authorize(TestJwtIssuer.AcmeEditor());

        var payload = new
        {
            firstName = "Eve",
            lastName = "Hall",
            email = $"eve-{Guid.NewGuid():N}@acme.example",
            phoneNumber = "4155550199",
            tenantId = DemoTenants.ContosoFinanceId
        };

        var response = await _client.PostAsJsonAsync("/api/customers", payload);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();
        created!.Data.TenantId.Should().Be(DemoTenants.AcmeBankId);
        created.Data.TenantId.Should().NotBe(DemoTenants.ContosoFinanceId);
    }

    [Fact]
    public async Task WrongIssuer_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "acme-reader",
            "acme.reader",
            DemoTenants.AcmeBankId,
            DemoTenants.AcmeBankAlias,
            ["tenant-reader"],
            issuer: "https://evil.example/realms/other");

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WrongAudience_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "acme-reader",
            "acme.reader",
            DemoTenants.AcmeBankId,
            DemoTenants.AcmeBankAlias,
            ["tenant-reader"],
            audience: "some-other-api");

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExpiredToken_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "acme-reader",
            "acme.reader",
            DemoTenants.AcmeBankId,
            DemoTenants.AcmeBankAlias,
            ["tenant-reader"],
            expires: DateTime.UtcNow.AddMinutes(-5));

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_DoesNotReturnAccessToken()
    {
        Authorize(TestJwtIssuer.AcmeReader());

        var response = await _client.GetAsync("/api/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotContain("Bearer");
        body.Should().NotContain("eyJ");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<MeResponse>>();
        result!.Data.Subject.Should().Be("acme-reader");
        result.Data.TenantId.Should().Be(DemoTenants.AcmeBankId);
        result.Data.Roles.Should().Contain("tenant-reader");
    }

    [Fact]
    public async Task SystemPing_RemainsAnonymous()
    {
        var response = await _client.GetAsync("/api/system/ping");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("pong");
    }

    [Fact]
    public async Task AmbiguousOrganization_IsRejected()
    {
        var organization = $"{{\"{DemoTenants.AcmeBankAlias}\":{{\"id\":\"{DemoTenants.AcmeBankId}\"}},\"{DemoTenants.ContosoFinanceAlias}\":{{\"id\":\"{DemoTenants.ContosoFinanceId}\"}}}}";
        var token = CreateRawToken(organization, """{"customer-api":{"roles":["tenant-reader"]}}""");

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private void Authorize(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static CreateCustomerRequest NewCustomer(string email) =>
        new()
        {
            FirstName = "New",
            LastName = "Customer",
            Email = email,
            PhoneNumber = "4155550100"
        };

    private static string CreateRawToken(string organizationJson, string resourceAccessJson)
    {
        var claims = new[]
        {
            new System.Security.Claims.Claim("sub", "multi-org"),
            new System.Security.Claims.Claim("preferred_username", "multi.org"),
            new System.Security.Claims.Claim("organization", organizationJson, "JSON"),
            new System.Security.Claims.Claim("resource_access", resourceAccessJson, "JSON")
        };

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: TestJwtIssuer.Issuer,
            audience: TestJwtIssuer.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                TestJwtIssuer.SigningKey,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256));

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}
