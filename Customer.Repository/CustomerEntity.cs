using System;

namespace Customer.Repository
{
	public class CustomerEntity
	{
        public Guid Id { get; set; }

        public string FirstName { get; set; } = default!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;
    }
}

