using AutoMapper;
using Customer.Common.Models.Customer;
using Customer.Repository;
using Customer.Repository.Interface;
using Customer.Service.Interface;
using Microsoft.Extensions.Logging;

namespace Customer.Service
{
    public class CustomerService : ICustomerService
    {
        private ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public readonly ILogger _logger;

        public CustomerService(ICustomerRepository customerRepository,
            IMapper mapper,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerResponse>
            AddAsync(CreateCustomerRequest customerRequest)
        {
            try
            {
                _logger.LogInformation($"Creating new customer started with email : {customerRequest.Email}");
                var customerEntity = _mapper.Map<CustomerEntity>(customerRequest);
                var saveCustomerResponse = await _customerRepository.AddAsync(customerEntity);
                _logger.LogInformation($"Creating new customer completed with email : {customerRequest.Email}");
                return _mapper.Map<CustomerResponse>(saveCustomerResponse);
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occured while creating customer with an email: {customerRequest.Email}, Exception: {ex}");
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
                _logger.LogError($"An error occured while deleting a customer with a customer id: {id}, Exception: {ex}");
                throw;
            }
        }

        public async Task<IEnumerable<CustomerResponse>?> GetAllAsync()
        {
            try
            { 
                var customersEntity = await _customerRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<CustomerResponse>>(customersEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while getting all the customers, Exception: {ex}");
                throw;
            }
        }

        public async Task<CustomerResponse?> GetByIdAsync(Guid id)
        {
            try
            { 
                var customerEntity = await _customerRepository.GetByIdAsync(id);
                if (customerEntity == null)
                {
                    throw new KeyNotFoundException($"Customer with id: {id} does not exist. Please try a valid customer id.");
                }
                return _mapper.Map<CustomerResponse>(customerEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while getting a customer with a customer id: {id}, Exception: {ex}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateCustomerRequest updateCustomerRequest, Guid id)
        {
            try
            { 
                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if(existingCustomer == null)
                {
                    throw new KeyNotFoundException($"Customer with id: {id} does not exist to update");
                }

                existingCustomer.FirstName = !string.IsNullOrEmpty(updateCustomerRequest.FirstName)
                                && updateCustomerRequest.FirstName != existingCustomer.FirstName
                                ? updateCustomerRequest.FirstName : existingCustomer.FirstName;

                existingCustomer.MiddleName = !string.IsNullOrEmpty(updateCustomerRequest.MiddleName)
                    && updateCustomerRequest.MiddleName != existingCustomer.MiddleName
                    ? updateCustomerRequest.MiddleName : existingCustomer.MiddleName;

                existingCustomer.LastName = !string.IsNullOrEmpty(updateCustomerRequest.LastName)
                    && updateCustomerRequest.LastName != existingCustomer.LastName
                    ? updateCustomerRequest.LastName : existingCustomer.LastName;

                existingCustomer.Email = !string.IsNullOrEmpty(updateCustomerRequest.Email)
                    && updateCustomerRequest.Email != existingCustomer.Email
                    ? updateCustomerRequest.Email : existingCustomer.Email;

                existingCustomer.CountryCode = !string.IsNullOrEmpty(updateCustomerRequest.CountryCode)
                    && updateCustomerRequest.CountryCode != existingCustomer.CountryCode
                    ? updateCustomerRequest.CountryCode : existingCustomer.CountryCode;

                existingCustomer.AreaCode = !string.IsNullOrEmpty(updateCustomerRequest.AreaCode)
                    && updateCustomerRequest.AreaCode != existingCustomer.AreaCode
                    ? updateCustomerRequest.AreaCode : existingCustomer.AreaCode;

                existingCustomer.PhoneNumber = !string.IsNullOrEmpty(updateCustomerRequest.PhoneNumber)
                                            && updateCustomerRequest.PhoneNumber != existingCustomer.PhoneNumber
                                            ? updateCustomerRequest.PhoneNumber : existingCustomer.PhoneNumber;

                return await _customerRepository.UpdateAsync(existingCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while updating a customer with a customer id: {id}, Exception: {ex}");
                throw;
            }
        }
    }
}