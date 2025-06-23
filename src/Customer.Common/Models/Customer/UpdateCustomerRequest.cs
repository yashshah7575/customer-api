using System;
namespace Customer.Common.Models.Customer
{
    public class UpdateCustomerRequest
    {
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? CountryCode { get; set; }

        public string? AreaCode { get; set; }

        public string? PhoneNumber { get; set; }
    }
}