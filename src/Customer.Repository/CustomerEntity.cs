namespace Customer.Repository;

public class CustomerEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Keycloak organization ID. Always assigned by the server from ITenantContext.
    /// </summary>
    public string TenantId { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? CountryCode { get; set; }

    public string PhoneNumber { get; set; } = default!;
}
