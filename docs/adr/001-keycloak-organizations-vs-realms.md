# 001. One shared realm versus realm-per-tenant

## Context

B2B products often isolate tenants in the identity provider. Common Keycloak patterns include a realm per tenant, Keycloak Organizations, or a tenant claim on users and clients.

This API is a shared multi-tenant resource server with two demo tenants and a single operational team.

## Decision

Use **one realm** named `customer-api-demo`. Represent tenants with a stable `tenant_id` claim (`customer-a`, `customer-b`) mapped from user attributes (humans) or a client mapper (machine identity).

Keycloak Organizations remain a valid B2B option when users must pick an organization at login. This demo prefers a flat `tenant_id` claim so the application contract stays simple.

## Alternatives considered

**Realm per tenant.** Strongest isolation and independent signing keys. Operational cost grows linearly: JWKS endpoints, realm versioning, and user federation must be automated. The API would need dynamic issuer validation.

**Keycloak Organizations.** Useful when a user belongs to multiple customers and selects one at login. Extra token shape (`organization` JSON) and Admin UI membership steps. Not required for this demonstration.

## Consequences

- One Authority and one JWKS URL for the API.
- Tokens carry at most one `tenant_id`. Conflicting claims are rejected.
- Very large or regulated tenants that need separate crypto would still justify realm-per-tenant later.
