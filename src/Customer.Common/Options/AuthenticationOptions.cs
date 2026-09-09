namespace Customer.Common.Options;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Authority { get; set; } = "http://localhost:8080/realms/customer-platform";

    public string Audience { get; set; } = "customer-api";
}
