namespace Customer.Common.Tenancy;

/// <summary>
/// Local-development tenant identifiers shared by realm import, seed data, and tests.
/// </summary>
public static class DemoTenants
{
    public const string CustomerA = "customer-a";
    public const string CustomerB = "customer-b";

    public static readonly Guid CustomerAPrimaryId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid CustomerASecondaryId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid CustomerBPrimaryId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid CustomerBSecondaryId = Guid.Parse("20000000-0000-0000-0000-000000000002");
}
