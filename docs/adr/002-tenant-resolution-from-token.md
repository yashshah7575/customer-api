# 002. Tenant resolution from the access token

## Context

The API must know the current tenant before it reads or writes customer data. Incoming HTTP is untrusted. Clients can send any header, route value, or JSON field.

## Decision

Resolve tenant **only** from a validated access token:

- `TenantId` = `tenant_id` claim
- Require exactly one tenant for non-platform tenant-scoped operations
- `PlatformAdmin` may omit `tenant_id` for cross-tenant reads

Do not read tenant identity from headers, routes, query strings, or bodies.

## Alternatives considered

**`X-Tenant-Id` header.** Convenient for browsers and gateways. Trivially forgeable once a token is obtained.

**Tenant in the URL (`/api/{tenant}/customers`).** Useful for cache keys and support tooling. Still must be authorized against the token. If the URL disagrees with the token, the safe behavior is reject, which makes the URL redundant for security.

**Trust `azp` or username conventions.** Brittle and not a tenant identifier.

## Consequences

- Protocol mappers must emit `tenant_id` on human and machine tokens.
- Ambiguous tokens receive 403.
- Claim parsing is centralized so controllers never inspect raw JWT JSON.
