using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Customer.Common;
using Customer.Common.Models.Customer;
using FluentAssertions;

namespace Customer.Api.Integration.Tests;

public class CustomerApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomerApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwtIssuer.AcmeEditor());
    }

    [Fact]
    public async Task AddCustomer_ShouldReturnCreatedCustomer()
    {
        var request = new CreateCustomerRequest
        {
            FirstName = "NUnit",
            LastName = "User",
            Email = $"nunit-{Guid.NewGuid():N}@acme.example",
            PhoneNumber = "1234567890"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();
        result!.Data.FirstName.Should().Be("NUnit User".Split(' ')[0]);
        result.Data.FirstName.Should().Be("NUnit");
        result.Data.TenantId.Should().Be(Customer.Common.Tenancy.DemoTenants.AcmeBankId);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnTenantCustomers()
    {
        var request = new CreateCustomerRequest
        {
            FirstName = "Test",
            LastName = "Fetch",
            Email = $"fetch-{Guid.NewGuid():N}@acme.example",
            PhoneNumber = "5555555555"
        };

        await _client.PostAsJsonAsync("/api/customers", request);

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerResponse>>>();
        result!.Data.Should().Contain(customer => customer.Email == request.Email);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnTrue()
    {
        var request = new CreateCustomerRequest
        {
            FirstName = "To",
            LastName = "Delete",
            Email = $"delete-{Guid.NewGuid():N}@acme.example",
            PhoneNumber = "9999999999"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponseData<CustomerResponse>>();

        var response = await _client.DeleteAsync($"/api/customers/{created!.Data.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<bool>>();
        result!.Data.Should().BeTrue();
    }
}
