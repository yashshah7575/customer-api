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
                Id = DemoTenants.AcmeAliceId,
                TenantId = DemoTenants.AcmeBankId,
                FirstName = "Alice",
                LastName = "Nguyen",
                Email = "alice.nguyen@acme-bank.example",
                CountryCode = "+1",
                PhoneNumber = "4155550101"
            },
            new CustomerEntity
            {
                Id = DemoTenants.AcmeBobId,
                TenantId = DemoTenants.AcmeBankId,
                FirstName = "Bob",
                LastName = "Patel",
                Email = "bob.patel@acme-bank.example",
                CountryCode = "+1",
                PhoneNumber = "4155550102"
            },
            new CustomerEntity
            {
                Id = DemoTenants.ContosoCarolId,
                TenantId = DemoTenants.ContosoFinanceId,
                FirstName = "Carol",
                LastName = "Diaz",
                Email = "carol.diaz@contoso-finance.example",
                CountryCode = "+1",
                PhoneNumber = "2065550101"
            },
            new CustomerEntity
            {
                Id = DemoTenants.ContosoDaveId,
                TenantId = DemoTenants.ContosoFinanceId,
                FirstName = "Dave",
                LastName = "Okoye",
                Email = "dave.okoye@contoso-finance.example",
                CountryCode = "+1",
                PhoneNumber = "2065550102"
            });

        await dbContext.SaveChangesAsync();
    }
}
