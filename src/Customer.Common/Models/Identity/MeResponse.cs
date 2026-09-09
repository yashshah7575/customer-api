namespace Customer.Common.Models.Identity;

public sealed class MeResponse
{
    public string? Subject { get; set; }

    public string? Username { get; set; }

    public string? TenantId { get; set; }

    public string? TenantAlias { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } = [];
}
