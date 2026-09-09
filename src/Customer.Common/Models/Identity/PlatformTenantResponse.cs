namespace Customer.Common.Models.Identity;

public sealed class PlatformTenantResponse
{
    public required string TenantId { get; set; }

    public required string Alias { get; set; }

    public required string Name { get; set; }
}
