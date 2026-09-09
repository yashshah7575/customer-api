using Customer.Common.Identity;
using Customer.Common.Tenancy;
using Customer.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository.Tests;

public class CustomerRepositoryTests : IDisposable
{
    private readonly TenantContext _tenantContext = new()
    {
        TenantId = DemoTenants.AcmeBankId,
        TenantAlias = DemoTenants.AcmeBankAlias,
        TenantStatus = TenantResolutionStatus.Valid,
        IsAuthenticated = true
    };

    private readonly CustomerDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new CustomerDbContext(options, _tenantContext);
        _repository = new CustomerRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_ShouldAddCustomer()
    {
        var customer = CreateCustomer("John", "Doe", "john@acme.example");

        var result = await _repository.AddAsync(customer);

        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.TenantId.Should().Be(DemoTenants.AcmeBankId);

        var dbCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == result.Id);
        dbCustomer.Should().NotBeNull();
        dbCustomer!.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenExists()
    {
        var customer = CreateCustomer("Jane", "Smith", "jane@acme.example");
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(customer.Id);

        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerBelongsToAnotherTenant()
    {
        var foreignCustomer = CreateCustomer("Carol", "Diaz", "carol@contoso.example", DemoTenants.ContosoFinanceId);
        _context.Customers.Add(foreignCustomer);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(foreignCustomer.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyCurrentTenantCustomers()
    {
        _context.Customers.AddRange(
            CreateCustomer("A", "A", "a@acme.example"),
            CreateCustomer("B", "B", "b@acme.example"),
            CreateCustomer("C", "C", "c@contoso.example", DemoTenants.ContosoFinanceId));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(customer => customer.TenantId == DemoTenants.AcmeBankId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyCustomer()
    {
        var customer = CreateCustomer("fname", "lname", "edit@acme.example");
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        customer.FirstName = "New";
        var result = await _repository.UpdateAsync(customer);

        result.Should().BeTrue();
        var dbCustomer = await _context.Customers.FirstAsync(c => c.Id == customer.Id);
        dbCustomer.FirstName.Should().Be("New");
        dbCustomer.TenantId.Should().Be(DemoTenants.AcmeBankId);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCustomer_WhenExists()
    {
        var customer = CreateCustomer("Del", "lname", "del@acme.example");
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(customer.Id);

        result.Should().BeTrue();
        (await _context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenCustomerBelongsToAnotherTenant()
    {
        var foreignCustomer = CreateCustomer("Carol", "Diaz", "carol-del@contoso.example", DemoTenants.ContosoFinanceId);
        _context.Customers.Add(foreignCustomer);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(foreignCustomer.Id);

        result.Should().BeFalse();
        (await _context.Customers.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == foreignCustomer.Id))
            .Should().NotBeNull();
    }

    private static CustomerEntity CreateCustomer(
        string firstName,
        string lastName,
        string email,
        string? tenantId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId ?? DemoTenants.AcmeBankId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = "123456"
        };
}
