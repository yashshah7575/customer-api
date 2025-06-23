using System;
namespace Customer.Common.Models.Customer
{
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; } = default!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? CountryCode { get; set; }   

        public string? AreaCode { get; set; }

        public string PhoneNumber { get; set; } = default!;
    } 
}