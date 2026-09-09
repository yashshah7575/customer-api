using Customer.Common.Identity;
using Customer.Common.Models.Customer;
using Customer.Common.Tenancy;
using Customer.Repository;
using Customer.Repository.Interface;
using Customer.Service;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Customer.Service.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repo = new();
    private readonly Mock<ILogger<CustomerService>> _logger = new();
    private readonly TenantContext _tenantContext = new()
    {
        TenantId = DemoTenants.CustomerA,
        TenantStatus = TenantResolutionStatus.Valid,
        IsAuthenticated = true
    };

    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerService = new CustomerService(_repo.Object, _tenantContext, _logger.Object);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_AssignsTenantIdFromContext()
    {
        var req = new CreateCustomerRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@acme.example",
            PhoneNumber = "123456"
        };

        _repo.Setup(r => r.AddAsync(It.Is<CustomerEntity>(c => c.TenantId == DemoTenants.CustomerA)))
            .ReturnsAsync((CustomerEntity customer) =>
            {
                customer.Id = Guid.NewGuid();
                return customer;
            });

        var result = await _customerService.AddAsync(req);

        result.Email.Should().Be(req.Email);
        result.TenantId.Should().Be(DemoTenants.CustomerA);
        _repo.Verify(r => r.AddAsync(It.Is<CustomerEntity>(c => c.TenantId == DemoTenants.CustomerA)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithoutTenant_Throws()
    {
        _tenantContext.TenantId = null;
        _tenantContext.TenantStatus = TenantResolutionStatus.None;
        _tenantContext.IsPlatformAdmin = true;

        var act = async () => await _customerService.AddAsync(new CreateCustomerRequest
        {
            FirstName = "Pat",
            LastName = "Platform",
            Email = "pat@platform.example",
            PhoneNumber = "1"
        });

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddAsync_WhenRepositoryThrows_Rethrows()
    {
        var req = new CreateCustomerRequest { Email = "boom@acme.example", FirstName = "A", LastName = "B", PhoneNumber = "1" };
        _repo.Setup(r => r.AddAsync(It.IsAny<CustomerEntity>())).ThrowsAsync(new InvalidOperationException());

        var act = async () => await _customerService.AddAsync(req);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_ForwardsReturnValue()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _customerService.DeleteAsync(id);

        result.Should().BeTrue();
        _repo.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedCollection()
    {
        var entities = new List<CustomerEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TenantId = DemoTenants.CustomerA,
                FirstName = "A",
                LastName = "B",
                Email = "a@acme.example",
                PhoneNumber = "1"
            }
        };

        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = await _customerService.GetAllAsync();

        result.Should().ContainSingle(customer => customer.Id == entities[0].Id && customer.TenantId == DemoTenants.CustomerA);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsMappedCustomer()
    {
        var id = Guid.NewGuid();
        var ent = new CustomerEntity
        {
            Id = id,
            TenantId = DemoTenants.CustomerA,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@acme.example",
            PhoneNumber = "1"
        };

        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(ent);

        var result = await _customerService.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.TenantId.Should().Be(DemoTenants.CustomerA);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CustomerEntity?)null);

        var act = async () => await _customerService.GetByIdAsync(id);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Customer with id: {id} does not exist*");
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CustomerEntity?)null);

        var act = async () => await _customerService.UpdateAsync(new UpdateCustomerRequest(), id);
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Customer with id: {id} does not exist to update");
    }
}
