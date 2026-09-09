# 002. Tenant resolution from the access token

## Context

The API must know the current tenant before it reads or writes customer data. Incoming HTTP is untrusted. Clients can send any header, route value, or JSON field.

Keycloak Organizations can emit an `organization` claim containing alias and id when the `organization` scope is requested.

## Decision

Resolve tenant **only** from a validated access token:

- `TenantId` = organization id
- `TenantAlias` = organization alias
- Require exactly one organization for tenant-scoped operations

Do not read tenant identity from headers, routes, query strings, or bodies.

## Alternatives considered

**`X-Tenant-Id` header.** Convenient for browsers and gateways. Trivially forgeable once a token is obtained.

**Tenant in the URL (`/api/{tenant}/customers`).** Useful for cache keys and support tooling. Still must be authorized against the token. If the URL disagrees with the token, the safe behavior is reject, which makes the URL redundant for security.

**Trust `azp` or username conventions.** Brittle and not a tenant identifier.

## Consequences

- Swagger and other clients must request the `organization` scope.
- Multi-org users must select an organization at login or receive 403.
- Platform administrators can operate without an organization on platform routes only.
- Claim parsing is centralized so controllers never inspect raw JWT JSON.
