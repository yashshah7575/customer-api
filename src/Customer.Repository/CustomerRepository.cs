using Customer.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Customer.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _dbContext;
        private ILogger<CustomerRepository> _logger;

        public CustomerRepository(CustomerDbContext dbContext, ILogger<CustomerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<CustomerEntity?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation($"Getting Customer my id: {id}");
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        {
            _logger.LogInformation($"Getting all customer");
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<CustomerEntity>
            AddAsync(CustomerEntity customer)
        {
            _logger.LogInformation($"Saving customer details to the DB.");
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
                _logger.LogError($"Saving customer details to the DB.{customer}");
                return false;
            }

            var customerWithEmail = await GetCustomerByEmail(customer.Email);
            if (customerWithEmail != null && customerWithEmail.Id != existing.Id)
            {
                _logger.LogError($"Customer with same email and different id exists.");
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
            _logger.LogError($"Deleting customer with the id.{id}");
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CustomerEntity?> GetCustomerByEmail(string email)
        {
            return await _dbContext.Customers.Where(a =>
                        string.Equals(a.Email, email, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefaultAsync();
        }
    }
}