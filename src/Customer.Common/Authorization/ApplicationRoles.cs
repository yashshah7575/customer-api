namespace Customer.Common.Authorization;

/// <summary>
/// Application roles issued as Keycloak client roles on <c>customer-api</c>.
/// Realm roles are not used for API authorization.
/// </summary>
public static class ApplicationRoles
{
    public const string PlatformAdmin = "PlatformAdmin";
    public const string CustomerAdmin = "CustomerAdmin";
    public const string Operator = "Operator";
    public const string Viewer = "Viewer";
    public const string ServiceClient = "ServiceClient";

    public static IReadOnlySet<string> PermissionsFor(IEnumerable<string> roles)
    {
        var permissions = new HashSet<string>(StringComparer.Ordinal);

        foreach (var role in roles)
        {
            switch (role)
            {
                case Viewer:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    break;
                case Operator:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    permissions.Add(ApplicationPermissions.CustomersManage);
                    break;
                case CustomerAdmin:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    permissions.Add(ApplicationPermissions.CustomersManage);
                    permissions.Add(ApplicationPermissions.CustomersDelete);
                    break;
                case ServiceClient:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    break;
                case PlatformAdmin:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    permissions.Add(ApplicationPermissions.CustomersManage);
                    permissions.Add(ApplicationPermissions.CustomersDelete);
                    permissions.Add(ApplicationPermissions.PlatformAdminister);
                    break;
            }
        }

        return permissions;
    }
}
