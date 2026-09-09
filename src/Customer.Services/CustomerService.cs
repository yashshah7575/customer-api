using Customer.Common.Identity;
using Customer.Common.Models.Customer;
using Customer.Repository.Interface;
using Customer.Service.Interface;
using Microsoft.Extensions.Logging;

namespace Customer.Service;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        ITenantContext tenantContext,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<CustomerResponse> AddAsync(CreateCustomerRequest customerRequest)
    {
        if (!_tenantContext.HasValidTenant || string.IsNullOrWhiteSpace(_tenantContext.TenantId))
        {
            throw new InvalidOperationException("A single tenant context is required to create a customer.");
        }

        try
        {
            _logger.LogInformation("Creating customer started for {Email}", customerRequest.Email);
            var customerEntity = CustomerMapper.ToEntity(customerRequest);
            customerEntity.TenantId = _tenantContext.TenantId;
            var savedCustomer = await _customerRepository.AddAsync(customerEntity);
            _logger.LogInformation("Creating customer completed for {Email}", customerRequest.Email);
            return CustomerMapper.ToResponse(savedCustomer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a customer with email {Email}", customerRequest.Email);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            return await _customerRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting customer {CustomerId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerResponse>?> GetAllAsync()
    {
        try
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(CustomerMapper.ToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting customers");
            throw;
        }
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            var customerEntity = await _customerRepository.GetByIdAsync(id);
            if (customerEntity is null)
            {
                _logger.LogWarning(
                    "Customer {CustomerId} was not visible to tenant {TenantId} (platformAdmin={IsPlatformAdmin})",
                    id,
                    _tenantContext.TenantId,
                    _tenantContext.IsPlatformAdmin);
                throw new KeyNotFoundException($"Customer with id: {id} does not exist. Please try a valid customer id.");
            }

            return CustomerMapper.ToResponse(customerEntity);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting customer {CustomerId}", id);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(UpdateCustomerRequest updateCustomerRequest, Guid id)
    {
        try
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer is null)
            {
                throw new KeyNotFoundException($"Customer with id: {id} does not exist to update");
            }

            existingCustomer.FirstName = !string.IsNullOrEmpty(updateCustomerRequest.FirstName)
                && updateCustomerRequest.FirstName != existingCustomer.FirstName
                ? updateCustomerRequest.FirstName
                : existingCustomer.FirstName;

            existingCustomer.MiddleName = !string.IsNullOrEmpty(updateCustomerRequest.MiddleName)
                && updateCustomerRequest.MiddleName != existingCustomer.MiddleName
                ? updateCustomerRequest.MiddleName
                : existingCustomer.MiddleName;

            existingCustomer.LastName = !string.IsNullOrEmpty(updateCustomerRequest.LastName)
                && updateCustomerRequest.LastName != existingCustomer.LastName
                ? updateCustomerRequest.LastName
                : existingCustomer.LastName;

            existingCustomer.Email = !string.IsNullOrEmpty(updateCustomerRequest.Email)
                && updateCustomerRequest.Email != existingCustomer.Email
                ? updateCustomerRequest.Email
                : existingCustomer.Email;

            existingCustomer.CountryCode = !string.IsNullOrEmpty(updateCustomerRequest.CountryCode)
                && updateCustomerRequest.CountryCode != existingCustomer.CountryCode
                ? updateCustomerRequest.CountryCode
                : existingCustomer.CountryCode;

            existingCustomer.PhoneNumber = !string.IsNullOrEmpty(updateCustomerRequest.PhoneNumber)
                && updateCustomerRequest.PhoneNumber != existingCustomer.PhoneNumber
                ? updateCustomerRequest.PhoneNumber
                : existingCustomer.PhoneNumber;

            return await _customerRepository.UpdateAsync(existingCustomer);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating customer {CustomerId}", id);
            throw;
        }
    }
}
