namespace Customer.Common.Identity;

public interface IApplicationIdentity
{
    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Permissions { get; }

    bool HasPermission(string permission);
}
