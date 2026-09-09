# Identity architecture

This document describes how enterprise identity brokering fits the Customer API. The local demo does **not** connect Okta, Entra ID, or another paid provider.

## Demo versus target

```text
                   Enterprise Customer Identity

             Entra ID / Okta / SAML / OIDC
                         │
                         │
                 Identity Brokering
                         │
                         ▼
                 ┌──────────────────┐
                 │     Keycloak     │
                 │                  │
                 │ Authentication   │
                 │ OAuth2 / OIDC    │
                 │ Roles            │
                 │ Token Issuance   │
                 └────────┬─────────┘
                          │
                          │ JWT Access Token
                          ▼
                ┌───────────────────┐
                │   Customer API    │
                │     .NET API      │
                │                   │
                │ Authentication    │
                │ Authorization     │
                │ Tenant Isolation  │
                └─────────┬─────────┘
                          │
                          ▼
                   Application Data
```

Machine-to-machine:

```text
Customer Integration
        │
        │ OAuth2 Client Credentials
        ▼
     Keycloak
        │
        │ Access Token
        ▼
   Customer API
```

## Boundary

Keycloak is responsible for authentication, identity federation, token issuance, roles/scopes, and machine identity.

The Customer API is responsible for tenant isolation, resource ownership, domain authorization, and business rules.

Example:

```text
Keycloak says:

User = Alice
Tenant = CustomerA
Role = Operator

API determines:

Can Alice modify Customer #123?
Does Customer #123 belong to CustomerA?
Does Operator have permission for this operation?
```

## Brokering

```text
Customer Entra ID ───┐
Customer Okta ───────┼──→ Keycloak ──OIDC──→ Customer API
Customer SAML IdP ───┘
```

- Keycloak acts as the identity broker.
- Enterprise customers retain their own IdP.
- The application receives a normalized identity (`sub`, `tenant_id`, `roles`, `aud`).
- External groups or claims are mapped into application roles during IdP configuration, not inside controllers.
- Tenant identity is bound during onboarding (user attribute, client mapper, or organization mapping). It is never taken from an API request.
- The API does not care whether authentication originated from Okta, Entra, SAML, or another OIDC provider.

## Local demo mapping

The imported realm `customer-api-demo` simulates the **normalized** side of that contract:

- Human users authenticate with Authorization Code + PKCE (`customer-api-swagger`).
- The Customer A integration authenticates with Client Credentials (`customer-a-integration`).
- Access tokens carry `tenant_id` and `roles`, with audience `customer-api`.
