using Customer.Common.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository;

public static class CustomerDataSeeder
{
    public static async Task SeedAsync(CustomerDbContext dbContext)
    {
        if (await dbContext.Customers.IgnoreQueryFilters().AnyAsync())
        {
            return;
        }

        dbContext.Customers.AddRange(
            new CustomerEntity
            {
                Id = DemoTenants.CustomerAPrimaryId,
                TenantId = DemoTenants.CustomerA,
                FirstName = "Alice",
                LastName = "Nguyen",
                Email = "alice.nguyen@customer-a.example",
                CountryCode = "+1",
                PhoneNumber = "4155550101"
            },
            new CustomerEntity
            {
                Id = DemoTenants.CustomerASecondaryId,
                TenantId = DemoTenants.CustomerA,
                FirstName = "Aaron",
                LastName = "Patel",
                Email = "aaron.patel@customer-a.example",
                CountryCode = "+1",
                PhoneNumber = "4155550102"
            },
            new CustomerEntity
            {
                Id = DemoTenants.CustomerBPrimaryId,
                TenantId = DemoTenants.CustomerB,
                FirstName = "Bob",
                LastName = "Diaz",
                Email = "bob.diaz@customer-b.example",
                CountryCode = "+1",
                PhoneNumber = "2065550101"
            },
            new CustomerEntity
            {
                Id = DemoTenants.CustomerBSecondaryId,
                TenantId = DemoTenants.CustomerB,
                FirstName = "Bella",
                LastName = "Okoye",
                Email = "bella.okoye@customer-b.example",
                CountryCode = "+1",
                PhoneNumber = "2065550102"
            });

        await dbContext.SaveChangesAsync();
    }
}
