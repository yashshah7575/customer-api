using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Customer.Common;
using Customer.Common.Authorization;
using Customer.Common.Models.Customer;
using Customer.Common.Models.Identity;
using Customer.Common.Tenancy;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

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
    public async Task Viewer_CannotWrite()
    {
        Authorize(TestJwtIssuer.BobViewer());

        var create = await _client.PostAsJsonAsync("/api/customers", NewCustomer("viewer@customer-b.example"));
        create.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var update = await _client.PutAsJsonAsync(
            $"/api/customers/{DemoTenants.CustomerBPrimaryId}",
            new UpdateCustomerRequest { FirstName = "Nope" });
        update.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Operator_CanCreateWithinTenant()
    {
        Authorize(TestJwtIssuer.AliceOperator());

        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            NewCustomer($"operator-{Guid.NewGuid():N}@customer-a.example"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();
        created!.Data.TenantId.Should().Be(DemoTenants.CustomerA);
    }

    [Fact]
    public async Task Operator_CannotDelete()
    {
        Authorize(TestJwtIssuer.AliceOperator());

        var response = await _client.DeleteAsync($"/api/customers/{DemoTenants.CustomerAPrimaryId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CustomerAdmin_CanDeleteWithinTenant()
    {
        Authorize(TestJwtIssuer.AliceAdmin());

        var create = await _client.PostAsJsonAsync(
            "/api/customers",
            NewCustomer($"admin-del-{Guid.NewGuid():N}@customer-a.example"));
        var created = await create.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();

        var response = await _client.DeleteAsync($"/api/customers/{created!.Data.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CustomerA_SeesOnlyCustomerA()
    {
        Authorize(TestJwtIssuer.AliceAdmin());

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        result!.Data.Should().NotBeEmpty();
        result.Data.Should().OnlyContain(customer => customer.TenantId == DemoTenants.CustomerA);
        result.Data.Should().Contain(customer => customer.Id == DemoTenants.CustomerAPrimaryId);
    }

    [Fact]
    public async Task CustomerB_SeesOnlyCustomerB()
    {
        Authorize(TestJwtIssuer.BobViewer());

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        result!.Data.Should().OnlyContain(customer => customer.TenantId == DemoTenants.CustomerB);
        result.Data.Should().Contain(customer => customer.Id == DemoTenants.CustomerBPrimaryId);
    }

    [Fact]
    public async Task CustomerA_CannotReadCustomerBById()
    {
        Authorize(TestJwtIssuer.AliceAdmin());

        var response = await _client.GetAsync($"/api/customers/{DemoTenants.CustomerBPrimaryId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PlatformAdmin_CanReadAcrossTenants()
    {
        Authorize(TestJwtIssuer.PlatformAdmin());

        var list = await _client.GetAsync("/api/customers");
        list.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await list.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        customers!.Data.Select(c => c.TenantId).Should().Contain(DemoTenants.CustomerA);
        customers.Data.Select(c => c.TenantId).Should().Contain(DemoTenants.CustomerB);

        var otherTenant = await _client.GetAsync($"/api/customers/{DemoTenants.CustomerBPrimaryId}");
        otherTenant.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PlatformAdmin_CanAccessPlatformCatalog()
    {
        Authorize(TestJwtIssuer.PlatformAdmin());

        var response = await _client.GetAsync("/api/platform/tenants");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CustomerAdmin_CannotAccessPlatformCatalog()
    {
        Authorize(TestJwtIssuer.AliceAdmin());

        var response = await _client.GetAsync("/api/platform/tenants");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ClientSuppliedTenantId_IsIgnoredOnCreate()
    {
        Authorize(TestJwtIssuer.AliceOperator());

        var payload = new
        {
            firstName = "Eve",
            lastName = "Hall",
            email = $"eve-{Guid.NewGuid():N}@customer-a.example",
            phoneNumber = "4155550199",
            tenantId = DemoTenants.CustomerB
        };

        var response = await _client.PostAsJsonAsync("/api/customers", payload);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();
        created!.Data.TenantId.Should().Be(DemoTenants.CustomerA);
    }

    [Fact]
    public async Task MissingTenantClaim_IsRejectedForTenantUsers()
    {
        var token = TestJwtIssuer.CreateToken("no-tenant", "no-tenant", null, [ApplicationRoles.Viewer]);
        Authorize(token);

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ServiceClient_CanReadOwnTenantOnly()
    {
        Authorize(TestJwtIssuer.ServiceClient());

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        result!.Data.Should().OnlyContain(customer => customer.TenantId == DemoTenants.CustomerA);

        var write = await _client.PostAsJsonAsync("/api/customers", NewCustomer("svc@customer-a.example"));
        write.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task WrongIssuer_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "alice-admin",
            "alice-admin",
            DemoTenants.CustomerA,
            [ApplicationRoles.CustomerAdmin],
            issuer: "https://evil.example/realms/other");

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WrongAudience_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "alice-admin",
            "alice-admin",
            DemoTenants.CustomerA,
            [ApplicationRoles.CustomerAdmin],
            audience: "some-other-api");

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WrongSigningKey_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "alice-admin",
            "alice-admin",
            DemoTenants.CustomerA,
            [ApplicationRoles.CustomerAdmin],
            signingKey: new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("other-test-signing-key-256-bit-min!")));

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExpiredToken_Returns401()
    {
        var token = TestJwtIssuer.CreateToken(
            "alice-admin",
            "alice-admin",
            DemoTenants.CustomerA,
            [ApplicationRoles.CustomerAdmin],
            expires: DateTime.UtcNow.AddMinutes(-5));

        Authorize(token);
        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_DoesNotReturnAccessToken()
    {
        Authorize(TestJwtIssuer.AliceAdmin());

        var response = await _client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotContain("Bearer");
        body.Should().NotContain("eyJ");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<MeResponse>>();
        result!.Data.Subject.Should().Be("alice-admin");
        result.Data.TenantId.Should().Be(DemoTenants.CustomerA);
        result.Data.Roles.Should().Contain(ApplicationRoles.CustomerAdmin);
    }

    [Fact]
    public async Task SystemPing_RemainsAnonymous()
    {
        var response = await _client.GetAsync("/api/system/ping");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private void Authorize(string token) =>
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    private static CreateCustomerRequest NewCustomer(string email) =>
        new()
        {
            FirstName = "New",
            LastName = "Customer",
            Email = email,
            PhoneNumber = "4155550100"
        };
}
