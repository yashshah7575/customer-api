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

            modelBuilder.Entity<CustomerEntity>()
                .Property(c => c.FirstName).IsRequired();

            modelBuilder.Entity<CustomerEntity>()
                .Property(c => c.LastName).IsRequired();

            modelBuilder.Entity<CustomerEntity>()
                .HasIndex(c => c.Email)
                .IsUnique(); // Enforce unique constraint

            modelBuilder.Entity<CustomerEntity>()
                .Property(c => c.MiddleName)
                .IsRequired(false); // Optional (nullable by default)

            modelBuilder.Entity<CustomerEntity>()
                .Property(c => c.CountryCode)
                .IsRequired(false); // Optional (nullable by default)

            modelBuilder.Entity<CustomerEntity>()
                .Property(c => c.PhoneNumber)
                .IsRequired()         // Not null
                .HasMaxLength(15);
        }
    }
}