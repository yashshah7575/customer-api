using Customer.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _dbContext;

        public CustomerRepository(CustomerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomerEntity?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<CustomerEntity>
            AddAsync(CustomerEntity customer)
        {
            customer.Id = Guid.NewGuid(); // ensure UUID is generated
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> UpdateAsync(CustomerEntity customer)
        {
            var existing = await _dbContext.Customers.FindAsync(customer.Id);
            if (existing == null)
            {
                return false;
            }
            existing.FirstName = customer.FirstName;
            existing.MiddleName = customer.MiddleName;
            existing.LastName = customer.LastName;
            existing.Email = customer.Email;
            existing.PhoneNumber = customer.PhoneNumber;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}