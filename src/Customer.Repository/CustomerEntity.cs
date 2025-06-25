using System;

namespace Customer.Repository
{
	public class CustomerEntity
	{
        public Guid Id { get; set; } = default!;

        public string FirstName { get; set; } = default!;

        public string? MiddleName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? CountryCode { get; set; } = default!;

        public string? AreaCode { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;
    }
}

