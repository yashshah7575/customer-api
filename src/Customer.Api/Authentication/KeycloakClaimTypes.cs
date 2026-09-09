namespace Customer.Api.Authentication;

public static class KeycloakClaimTypes
{
    public const string Subject = "sub";
    public const string PreferredUsername = "preferred_username";
    public const string Organization = "organization";
    public const string ResourceAccess = "resource_access";
    public const string Roles = "roles";
    public const string RealmAccess = "realm_access";
}
