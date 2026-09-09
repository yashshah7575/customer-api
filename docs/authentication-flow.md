# Authentication flow

## Roles

- **Keycloak** is the authorization server and OpenID Provider.
- **Customer API** is the resource server.
- **Swagger** is a public confidential-less OAuth client using Authorization Code + PKCE.

## Token validation

ASP.NET Core JWT Bearer middleware:

- Discovers metadata from `Authentication:Authority`
- Retrieves JWKS and validates the signature
- Validates issuer, audience, and lifetime
- Sets `MapInboundClaims=false` so code uses `sub`, `preferred_username`, `organization`, and `resource_access`

The application does not implement its own signature check.

`RequireHttpsMetadata` is disabled only in Development so a local Keycloak on HTTP works. Any other environment requires HTTPS metadata.

## Swagger

Client id: `customer-api-swagger`  
Public client, PKCE S256, no committed secret.

Requested scopes: `openid`, `profile`, `organization`.

An audience mapper on the Swagger client adds `customer-api` to the access token so the API's audience check succeeds.

## After authentication

`IdentityResolutionMiddleware` runs only for authenticated requests. It:

1. Copies `sub` and `preferred_username` into `ITenantContext`
2. Normalizes Keycloak client roles into application roles
3. Parses the organization claim
4. Rejects tenant-scoped endpoints when the organization claim is missing, malformed, or contains more than one organization

`GET /api/me` is authenticated but not tenant-scoped, so a platform administrator can inspect identity without an organization.
