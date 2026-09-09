namespace Customer.Common.Authorization;

public static class ApplicationRoles
{
    public const string PlatformAdmin = "platform-admin";
    public const string TenantAdmin = "tenant-admin";
    public const string TenantEditor = "tenant-editor";
    public const string TenantReader = "tenant-reader";

    public static IReadOnlySet<string> PermissionsFor(IEnumerable<string> roles)
    {
        var permissions = new HashSet<string>(StringComparer.Ordinal);

        foreach (var role in roles)
        {
            switch (role)
            {
                case TenantReader:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    break;
                case TenantEditor:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    permissions.Add(ApplicationPermissions.CustomersWrite);
                    break;
                case TenantAdmin:
                    permissions.Add(ApplicationPermissions.CustomersRead);
                    permissions.Add(ApplicationPermissions.CustomersWrite);
                    permissions.Add(ApplicationPermissions.TenantManage);
                    break;
                case PlatformAdmin:
                    permissions.Add(ApplicationPermissions.PlatformManage);
                    break;
            }
        }

        return permissions;
    }
}
