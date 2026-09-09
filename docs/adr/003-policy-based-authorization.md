# 003. Policy-based authorization

## Context

Keycloak can put roles on a realm or on a client. The API needs four capabilities: read customers, write customers, manage a tenant, and manage the platform.

Decorating every action with `[Authorize(Roles = "tenant-editor")]` couples HTTP to Keycloak naming and makes role-to-permission changes a search-and-replace exercise.

## Decision

Normalize Keycloak client roles into application permissions, then expose ASP.NET Core policies:

- `Customers.Read`
- `Customers.Write`
- `Tenant.Manage`
- `Platform.Manage`

Handlers check `IApplicationIdentity`, not claim JSON.

## Alternatives considered

**Role attributes on controllers.** Fast to write, expensive to evolve, and leaks IdP vocabulary into the API surface.

**Resource-based authorization per customer entity.** Correct for object-level ACLs, unnecessary for "all rows in the current tenant". Tenant isolation already happens in persistence.

**A general-purpose policy engine (OPA, Cedar).** Valuable at larger scale. Too much machinery for four permissions.

## Consequences

- Adding a permission is a mapping change plus a policy registration.
- `platform-admin` does not inherit tenant write access.
- Tests can mint tokens with client roles and assert HTTP 403/200 without Keycloak.
