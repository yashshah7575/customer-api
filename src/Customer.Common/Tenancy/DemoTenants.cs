namespace Customer.Common.Tenancy;

/// <summary>
/// Pinned organization identifiers shared by Keycloak realm import, demo seed data, and tests.
/// These values are local-development fixtures, not production tenant identifiers.
/// </summary>
public static class DemoTenants
{
    public const string AcmeBankId = "11111111-aaaa-4bbb-8ccc-111111111111";
    public const string AcmeBankAlias = "acme-bank";
    public const string AcmeBankName = "Acme Bank";

    public const string ContosoFinanceId = "22222222-aaaa-4bbb-8ccc-222222222222";
    public const string ContosoFinanceAlias = "contoso-finance";
    public const string ContosoFinanceName = "Contoso Finance";

    public static readonly Guid AcmeAliceId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid AcmeBobId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid ContosoCarolId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid ContosoDaveId = Guid.Parse("20000000-0000-0000-0000-000000000002");
}
