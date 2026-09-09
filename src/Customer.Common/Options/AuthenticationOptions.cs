namespace Customer.Common.Options;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Authority { get; set; } = "http://localhost:8080/realms/customer-api-demo";

    public string Audience { get; set; } = "customer-api";

    /// <summary>
    /// When unset, HTTPS metadata is required except in Development.
    /// </summary>
    public bool? RequireHttpsMetadata { get; set; }
}
