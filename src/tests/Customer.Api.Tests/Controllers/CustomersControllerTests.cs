using Customer.Api.Controllers;
using Customer.Common.Models.Customer;
using Customer.Service.Interface;
using FluentAssertions;
using Moq;

namespace Customer.Api.Tests.Controllers
{
    public class CustomersControllerTests
    {
        private Mock<ICustomerService> _customerServiceMock;
        private CustomersController _controller;

        [SetUp]
        public void SetUp()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _controller = new CustomersController(_customerServiceMock.Object);
        }

        [Test]
        public async Task GetCustomer_ShouldReturnAllCustomers()
        {
            // Arrange
            var mockCustomerData = new List<CustomerResponse>
            {
                new() { Id = Guid.NewGuid(), FirstName = "John" },
                new() { Id = Guid.NewGuid(), FirstName = "Jane" }
            };

            _customerServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(mockCustomerData);

            // Act
            var result = await _controller.GetCustomer();

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(mockCustomerData);
        }

        [Test]
        public async Task GetCustomerById_ShouldReturnCustomer_WhenFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockCustomerData = new CustomerResponse
            {
                Id = customerId,
                FirstName = "John"
            };

            _customerServiceMock
                .Setup(s => s.GetByIdAsync(customerId))
                .ReturnsAsync(mockCustomerData);

            // Act
            var result = await _controller.GetCustomerById(customerId);

            // Assert
            result.Data.Should().BeEquivalentTo(mockCustomerData);
        }

        [Test]
        public async Task AddCustomer_ShouldReturnCreatedCustomer()
        {
            // Arrange
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

            // Act
            var result = await _controller.AddCustomer(request);

            // Assert
            result.Data.Should().BeEquivalentTo(createdCustomer);
        }

        [Test]
        public async Task EditCustomer_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateRequest = new UpdateCustomerRequest
            {
                FirstName = "UpdatedName"
            };

            _customerServiceMock
                .Setup(s => s.UpdateAsync(updateRequest, customerId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.EditCustomer(updateRequest, customerId);

            // Assert
            result.Data.Should().BeTrue();
        }

        [Test]
        public async Task DeleteCustomer_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            _customerServiceMock
                .Setup(s => s.DeleteAsync(customerId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            result.Data.Should().BeTrue();
        }
    }
}