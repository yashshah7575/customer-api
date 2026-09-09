using Customer.Common.Identity;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository;

public class CustomerDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public DbSet<CustomerEntity> Customers { get; set; } = default!;

    public CustomerDbContext(DbContextOptions<CustomerDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var customer = modelBuilder.Entity<CustomerEntity>();

        customer.HasKey(c => c.Id);

        customer.Property(c => c.TenantId).IsRequired();
        customer.Property(c => c.FirstName).IsRequired();
        customer.Property(c => c.LastName).IsRequired();
        customer.Property(c => c.Email).IsRequired();
        customer.Property(c => c.MiddleName).IsRequired(false);
        customer.Property(c => c.CountryCode).IsRequired(false);
        customer.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        customer.HasIndex(c => new { c.TenantId, c.Email }).IsUnique();

        // Defense in depth: normal queries cannot see another tenant's rows.
        // Find/FindAsync bypass this filter — repositories must not use them.
        customer.HasQueryFilter(c =>
            _tenantContext.TenantId != null &&
            c.TenantId == _tenantContext.TenantId);
    }
}
