using Customer.Api.Controllers;
using Customer.Common;
using Customer.Common.Models.Customer;
using Customer.Service.Interface;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Customer.Api.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock = new();
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _controller = new CustomersController(_customerServiceMock.Object);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnAllCustomers()
    {
        var mockCustomerData = new List<CustomerResponse>
        {
            new() { Id = Guid.NewGuid(), FirstName = "John" },
            new() { Id = Guid.NewGuid(), FirstName = "Jane" }
        };

        _customerServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(mockCustomerData);

        var result = await _controller.GetCustomer();

        Unwrap(result).Data.Should().BeEquivalentTo(mockCustomerData);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnCustomer_WhenFound()
    {
        var customerId = Guid.NewGuid();
        var mockCustomerData = new CustomerResponse
        {
            Id = customerId,
            FirstName = "John"
        };

        _customerServiceMock
            .Setup(s => s.GetByIdAsync(customerId))
            .ReturnsAsync(mockCustomerData);

        var result = await _controller.GetCustomerById(customerId);

        Unwrap(result).Data.Should().BeEquivalentTo(mockCustomerData);
    }

    [Fact]
    public async Task AddCustomer_ShouldReturnCreatedCustomer()
    {
        var request = new CreateCustomerRequest
        {
            FirstName = "fname"
        };

        var createdCustomer = new CustomerResponse
        {
            Id = Guid.NewGuid(),
            FirstName = "fname"
        };

        _customerServiceMock
            .Setup(s => s.AddAsync(request))
            .ReturnsAsync(createdCustomer);

        var result = await _controller.AddCustomer(request);

        Unwrap(result).Data.Should().BeEquivalentTo(createdCustomer);
    }

    [Fact]
    public async Task EditCustomer_ShouldReturnTrue_WhenSuccessful()
    {
        var customerId = Guid.NewGuid();
        var updateRequest = new UpdateCustomerRequest
        {
            FirstName = "UpdatedName"
        };

        _customerServiceMock
            .Setup(s => s.UpdateAsync(updateRequest, customerId))
            .ReturnsAsync(true);

        var result = await _controller.EditCustomer(updateRequest, customerId);

        Unwrap(result).Data.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnTrue_WhenSuccessful()
    {
        var customerId = Guid.NewGuid();

        _customerServiceMock
            .Setup(s => s.DeleteAsync(customerId))
            .ReturnsAsync(true);

        var result = await _controller.DeleteCustomer(customerId);

        Unwrap(result).Data.Should().BeTrue();
    }

    private static ApiResponseData<T> Unwrap<T>(ActionResult<ApiResponseData<T>> result)
    {
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        return ok.Value.Should().BeOfType<ApiResponseData<T>>().Subject;
    }
}
