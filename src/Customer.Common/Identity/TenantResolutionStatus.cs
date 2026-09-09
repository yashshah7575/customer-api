namespace Customer.Common.Identity;

public enum TenantResolutionStatus
{
    None = 0,
    Valid = 1,
    Missing = 2,
    Ambiguous = 3,
    Malformed = 4
}
