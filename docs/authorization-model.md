# Authorization model

## Why policies instead of roles

Keycloak role names are an identity-provider detail. Controllers should ask "can this caller write customers?" not "is this `tenant-editor`?" Mapping roles to permissions in one place lets the IdP change without rewriting attributes on every action.

## Mapping

| Client role on `customer-api` | Permissions |
|---|---|
| `tenant-reader` | `Customers.Read` |
| `tenant-editor` | `Customers.Read`, `Customers.Write` |
| `tenant-admin` | `Customers.Read`, `Customers.Write`, `Tenant.Manage` |
| `platform-admin` | `Platform.Manage` |

`platform-admin` is intentionally **not** a superuser over tenant data. Platform administration is a different control plane. Cross-tenant Customer APIs are not exposed.

## Policy composition

Tenant-scoped policies require:

1. An authenticated user
2. The matching permission
3. A valid single tenant context

`Platform.Manage` requires authentication and the platform permission only.

## Where roles are read

`KeycloakRoleNormalizer` accepts:

- `resource_access.customer-api.roles` (preferred Keycloak client-role claim)
- a simple `roles` claim (useful in tests)
- `realm_access.roles` only if they match known application roles

Unknown roles are ignored. That prevents accidental privilege from leftover realm roles such as `offline_access`.
