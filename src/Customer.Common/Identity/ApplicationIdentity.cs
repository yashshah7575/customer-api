using Customer.Common.Authorization;

namespace Customer.Common.Identity;

public sealed class ApplicationIdentity : IApplicationIdentity
{
    private readonly HashSet<string> _roles = new(StringComparer.Ordinal);
    private readonly HashSet<string> _permissions = new(StringComparer.Ordinal);

    public IReadOnlyCollection<string> Roles => _roles;

    public IReadOnlyCollection<string> Permissions => _permissions;

    public void SetRoles(IEnumerable<string> roles)
    {
        _roles.Clear();
        _permissions.Clear();

        foreach (var role in roles)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                _roles.Add(role);
            }
        }

        foreach (var permission in ApplicationRoles.PermissionsFor(_roles))
        {
            _permissions.Add(permission);
        }
    }

    public bool HasPermission(string permission) => _permissions.Contains(permission);
}
