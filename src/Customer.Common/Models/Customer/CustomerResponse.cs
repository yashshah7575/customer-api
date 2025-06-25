namespace Customer.Common.Models.Customer
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = default!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? CountryCode { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;
    }
}