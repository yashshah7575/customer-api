# 001. Keycloak Organizations versus one realm per tenant

## Context

B2B products often isolate tenants in the identity provider. Two common Keycloak patterns are:

- **Realm per tenant** — each customer gets a realm, its own keys, themes, and IdP brokers.
- **Organization per tenant** — one realm, Organizations represent customers, users can belong to one or more organizations.

This API is a shared multi-tenant resource server with a small number of demo tenants and a single operational team.

## Decision

Use **one realm** named `customer-platform` and model tenants as **Keycloak Organizations**.

## Alternatives considered

**Realm per tenant.** Strongest isolation and independent signing keys. Operational cost grows linearly: JWKS endpoints, realm versioning, theming, and user federation must be automated. The API would need dynamic issuer validation or a realm-aware gateway.

**Group or custom attribute as tenant.** Groups are not a first-class B2B tenant primitive. They do not give organization-scoped login selection or the `organization` claim.

## Consequences

- One Authority and one JWKS URL for the API.
- Tokens can carry a single selected organization, which matches "exactly one tenant context".
- Users can theoretically be members of multiple organizations; the API rejects ambiguous tokens.
- Very large or regulated tenants that need separate crypto or compliance boundaries would still justify realm-per-tenant later.
