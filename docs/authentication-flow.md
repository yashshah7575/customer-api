# Authentication flow

## Human users

1. Swagger starts Authorization Code + PKCE against `customer-api-swagger`.
2. The user signs in at Keycloak realm `customer-api-demo`.
3. Keycloak returns an access token with `aud=customer-api`, `tenant_id`, and `roles`.
4. Swagger calls the API with `Authorization: Bearer`.

Requested scopes: `openid`, `profile`. Password grant is not used for this flow.

## Machines

`customer-a-integration` uses Client Credentials. The confidential client is bound to `customer-a` and `ServiceClient`.

## API validation

JWT Bearer middleware validates signature, issuer, audience, and lifetime from `Authentication` configuration.

`IdentityResolutionMiddleware` then:

1. Reads `sub` and `preferred_username`
2. Normalizes application roles (`roles`, with `resource_access.customer-api.roles` as fallback)
3. Parses a single `tenant_id` claim
4. Rejects tenant-scoped endpoints when a non-platform caller has a missing or conflicting tenant claim

`GET /api/me` is authenticated but not tenant-scoped, so `platform-admin` can inspect identity without a tenant.
