namespace Customer.Common.Models.Identity;

public sealed class TenantResponse
{
    public string? TenantId { get; set; }

    public string? TenantAlias { get; set; }
}
