namespace Customer.Common.Authorization;

public static class AuthorizationPolicies
{
    public const string CanReadCustomers = "CanReadCustomers";
    public const string CanManageCustomers = "CanManageCustomers";
    public const string CanDeleteCustomers = "CanDeleteCustomers";
    public const string PlatformAdministration = "PlatformAdministration";
}
