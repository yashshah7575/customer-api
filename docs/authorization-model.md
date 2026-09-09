# Authorization model

Keycloak role names are an identity-provider detail. Controllers ask named policies such as `CanManageCustomers`, not `if role == Operator`.

## Client roles

Roles are **application/client roles** on `customer-api`, not realm roles. That keeps the authorization boundary on this API instead of mixing in Keycloak built-in realm roles.

| Client role | Permissions |
|---|---|
| `Viewer` | `Customers.Read` |
| `Operator` | `Customers.Read`, `Customers.Manage` |
| `CustomerAdmin` | `Customers.Read`, `Customers.Manage`, `Customers.Delete` |
| `ServiceClient` | `Customers.Read` |
| `PlatformAdmin` | those customer permissions plus `Platform.Administer` |

## Policies

| Policy | Meaning |
|---|---|
| `CanReadCustomers` | Authenticated + read permission + tenant context (or PlatformAdmin) |
| `CanManageCustomers` | Create/update |
| `CanDeleteCustomers` | Delete (not Operator, Viewer, or ServiceClient) |
| `PlatformAdministration` | Platform catalog only |

Unknown roles are ignored so leftover Keycloak roles cannot grant application privileges.
