# 003. Policy-based authorization

## Context

Keycloak can put roles on a realm or on a client. Decorating every action with `[Authorize(Roles = "Operator")]` couples HTTP to IdP naming.

## Decision

Use **client roles** on `customer-api`, map them to application permissions, then expose ASP.NET Core policies:

- `CanReadCustomers`
- `CanManageCustomers`
- `CanDeleteCustomers`
- `PlatformAdministration`

Handlers check `IApplicationIdentity`, not claim JSON.

## Alternatives considered

**Role attributes on controllers.** Fast to write, expensive to evolve, and leaks IdP vocabulary into the API surface.

**Resource-based authorization per customer entity.** Correct for object-level ACLs, unnecessary for "all rows in the current tenant". Tenant isolation already happens in persistence.

**A general-purpose policy engine (OPA, Cedar).** Valuable at larger scale. Too much machinery for this sample.

## Consequences

- Adding a permission is a mapping change plus a policy registration.
- Operator cannot delete; Viewer cannot write; ServiceClient is read-only.
- `PlatformAdmin` cross-tenant access is explicit in the query filter and tests.
- Tests mint tokens with roles and assert HTTP 401/403/200 without Keycloak.
