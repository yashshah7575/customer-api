using System;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository
{
	public class CustomerDbContext : DbContext
	{
        public DbSet<CustomerEntity> Customers { get; set; }

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerEntity>()
                .HasKey(c => c.Id);
        }
    }
}