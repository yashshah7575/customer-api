using System.Net;
using System.Net.Http.Json;
using Customer.Api.Integration.Tests;
using Customer.Common;
using Customer.Repository;
using FluentAssertions;

namespace Customer.IntegrationTests;

[TestFixture]
public class CustomerApiTests
{
    private CustomWebApplicationFactory _factory;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task AddCustomer_ShouldReturnCreatedCustomer()
    {
        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "NUnit User",
            Email = "nunit@example.com",
            PhoneNumber = "1234567890"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", customer);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<CustomerEntity>>();
        result!.Data.FirstName.Should().Be("NUnit User");
    }

    [Test]
    public async Task GetCustomer_ShouldReturnAllCustomers()
    {
        // Add a customer first
        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Test Fetch",
            Email = "fetch@example.com",
            PhoneNumber = "5555555555"
        };

        await _client.PostAsJsonAsync("/api/customers", customer);

        var response = await _client.GetAsync("/api/customers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<IEnumerable<CustomerEntity>>>();
        result!.Data.Should().Contain(c => c.Email == "fetch@example.com");
    }

    [Test]
    public async Task DeleteCustomer_ShouldReturnTrue()
    {
        var customer = new CustomerEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "To Delete",
            Email = "delete@x.com",
            PhoneNumber = "9999999999"
        };

        await _client.PostAsJsonAsync("/api/customers", customer);

        var response = await _client.DeleteAsync($"/api/customers/{customer.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseData<bool>>();
        result!.Data.Should().BeTrue();
    }
}
