using Customer.Repository;
using Customer.Repository.Interface;
using Customer.Service.Interface;

namespace Customer.Service
{
    public class CustomerService : ICustomerService
    {
        private ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerEntity>
            AddAsync(CustomerEntity customer)
        {
            if (await IsUniqueCustomerEmail(customer.Email))
            {
                return await _customerRepository.AddAsync(customer);
            }
            return null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _customerRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<CustomerEntity?> GetByIdAsync(Guid id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(CustomerEntity customer)
        {
            return await _customerRepository.UpdateAsync(customer);
        }

        private async Task<bool> IsUniqueCustomerEmail(string email)
        {
            var customerEntity = await _customerRepository.GetCustomerByEmail(email);
            return customerEntity == null ? true : false;
        }
    }
}