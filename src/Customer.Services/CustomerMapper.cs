using Customer.Common.Models.Customer;
using Customer.Repository;

namespace Customer.Service;

public static class CustomerMapper
{
    public static CustomerEntity ToEntity(CreateCustomerRequest request) =>
        new()
        {
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            CountryCode = request.CountryCode,
            PhoneNumber = request.PhoneNumber
        };

    public static CustomerResponse ToResponse(CustomerEntity entity) =>
        new()
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            FirstName = entity.FirstName,
            MiddleName = entity.MiddleName,
            LastName = entity.LastName,
            Email = entity.Email,
            CountryCode = entity.CountryCode,
            PhoneNumber = entity.PhoneNumber
        };
}
