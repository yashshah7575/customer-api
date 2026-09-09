# Security design

Concise decisions for this sample. Local-only credentials in the realm import are throwaway demonstration values.

## Authentication

Keycloak + OIDC/OAuth2. The API is a resource server. Interactive users use Authorization Code + PKCE. Machines use Client Credentials. Resource Owner Password Credentials is not the primary user flow.

## JWT validation

JWT Bearer validates signature, issuer, audience, and lifetime from `Authentication` configuration. Unauthenticated callers are rejected by a fallback policy except for explicitly anonymous system routes. The API does not call Keycloak on every request. `RequireHttpsMetadata` is required outside Development.

## Tenant isolation

`tenant_id` is read from the validated access token. Request bodies, query strings, headers, and routes are not proof of tenant membership. Persistence uses an EF Core global query filter. Cross-tenant object access returns 404.

## Authorization

RBAC is coarse application permissioning:

- Keycloak issues client roles on `customer-api`: `PlatformAdmin`, `CustomerAdmin`, `Operator`, `Viewer`, `ServiceClient`.
- The API maps those roles to permissions, then to named policies (`CanReadCustomers`, `CanManageCustomers`, `CanDeleteCustomers`, `PlatformAdministration`).
- Domain and tenant rules stay in the application. Keycloak does not decide whether customer 123 belongs to Customer A.

## Machine identity

OAuth2 Client Credentials for `customer-a-integration`. The client is tenant-bound (`tenant_id=customer-a`) and limited to `ServiceClient` (read-only). It is not a platform administrator.

## Identity brokering

External enterprise IdPs terminate at Keycloak. See [identity-architecture.md](identity-architecture.md).

## Infrastructure

Keycloak configuration is version-controlled in `infra/keycloak/realm-export.json` and imported by Docker Compose. Kubernetes examples show API deployment concepts only.

## Security principle

Never trust network location or caller-provided tenant identity.
