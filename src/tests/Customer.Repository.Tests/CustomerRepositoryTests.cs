using Microsoft.EntityFrameworkCore;

namespace Customer.Repository.Tests
{
    [TestFixture]
    public class CustomerRepositoryTests
    {
        private CustomerDbContext _context;
        private CustomerRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            _context = new CustomerDbContext(options);
            _repository = new CustomerRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddCustomer()
        {
            var customer = new CustomerEntity
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "123456"
            };

            var result = await _repository.AddAsync(customer);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.Id);

            var dbCustomer = await _context.Customers.FindAsync(result.Id);
            Assert.IsNotNull(dbCustomer);
            Assert.AreEqual("John", dbCustomer.FirstName);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCustomer_WhenExists()
        {
            var customer = new CustomerEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "1234"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(customer.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Jane", result!.FirstName);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            var customer1 = new CustomerEntity { FirstName = "A", LastName = "A", Email = "a@a.com", PhoneNumber = "123" };
            var customer2 = new CustomerEntity { FirstName = "B", LastName = "B", Email = "b@b.com", PhoneNumber = "567" };

            _context.Customers.AddRange(customer1, customer2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyCustomer()
        {
            var customer = new CustomerEntity { FirstName = "fname", Email = "edit@me.com", LastName = "lname", PhoneNumber = "123" };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            customer.FirstName = "New";
            var result = await _repository.UpdateAsync(customer);

            Assert.IsTrue(result);

            var dbCustomer = await _context.Customers.FindAsync(customer.Id);
            Assert.AreEqual("New", dbCustomer!.FirstName);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveCustomer_WhenExists()
        {
            var customer = new CustomerEntity
            {
                FirstName = "Del",
                LastName = "lname",
                Email = "del@me.com",
                PhoneNumber = "123"
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteAsync(customer.Id);

            Assert.IsTrue(result);
            var dbCustomer = await _context.Customers.FindAsync(customer.Id);
            Assert.IsNull(dbCustomer);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
        {
            var result = await _repository.DeleteAsync(Guid.NewGuid());

            Assert.IsFalse(result);
        }
    }
}