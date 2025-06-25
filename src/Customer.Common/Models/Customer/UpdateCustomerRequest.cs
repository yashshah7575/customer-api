using System;
using Customer.Common.Models.Error;
using System.ComponentModel.DataAnnotations;

namespace Customer.Common.Models.Customer
{
    public class UpdateCustomerRequest
    {
        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }

        public string? CountryCode { get; set; }

        public string? PhoneNumber { get; set; }
    }
}