using System;
using Customer.Common.Models.Error;
using System.ComponentModel.DataAnnotations;

namespace Customer.Common.Models.Customer
{
    public class CreateCustomerRequest
    {
        [Required]
        public string FirstName { get; set; } = default!;

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; } = default!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = default!;

        public string? CountryCode { get; set; }   

        [Required]
        public string PhoneNumber { get; set; } = default!;


    } 
}