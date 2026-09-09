# Multi-tenancy

## Model

This API is **organization-per-tenant** inside one Keycloak realm (`customer-platform`). Organizations are the B2B tenant boundary.

Internal tenant key: Keycloak organization **ID**  
Display/logging key: organization **alias** (`acme-bank`, `contoso-finance`)

## Why the token is the only tenant source

If the client can send `X-Tenant-Id`, a path segment, or `tenantId` in JSON, any caller who obtains a valid token for tenant A can ask for tenant B's data. That is a forged-tenant and IDOR problem.

The server copies `TenantId` from `ITenantContext` when creating customers. Create and update DTOs do not expose a writable `TenantId`. Extra JSON properties are ignored.

## Ambiguity

A user may belong to multiple organizations. The `organization` scope asks Keycloak to map one organization, prompting the user when needed. If a token still contains more than one organization, tenant-scoped routes return 403.

Platform administrators may have no organization. They can call `GET /api/me` and `GET /api/platform/tenants`. They cannot list or mutate tenant customers.

## Persistence isolation

`CustomerEntity.TenantId` is required. `CustomerDbContext` applies:

```csharp
Customer.TenantId == ITenantContext.TenantId
```

Repositories use LINQ queries, not `FindAsync`, because `Find` bypasses global query filters in EF Core. Cross-tenant lookups therefore look like "not found" and become HTTP 404.

Email uniqueness is `(TenantId, Email)`, not global.

## Demo data

Seeded customers:

- Acme Bank: Alice Nguyen, Bob Patel
- Contoso Finance: Carol Diaz, Dave Okoye

Logging in as Acme never returns Contoso rows.
