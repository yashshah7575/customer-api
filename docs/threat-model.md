# Threat model

This is an application-level model for a multi-tenant resource server. It is not a full Keycloak operational review.

| Threat | What goes wrong | Mitigation |
|---|---|---|
| Forged TenantId | Caller puts another tenant in a header, route, or body | Tenant comes only from a validated organization claim. Body `tenantId` is ignored. Create uses `ITenantContext`. |
| Cross-tenant object access | Caller knows another tenant's customer GUID | Global query filter + no `FindAsync`. Missing rows return 404. |
| IDOR | Same as cross-tenant access via direct object reference | Isolation is persistence-level, not "remember to filter in the controller". |
| Invalid JWT | Unsigned or mutated token | JWT Bearer validates signature against Keycloak JWKS. No manual crypto. |
| Wrong issuer | Token from another realm or IdP | `ValidateIssuer` against configured Authority. |
| Wrong audience | Token minted for another API | `ValidateAudience` / `ValidAudience = customer-api`. |
| Expired access token | Replay after expiry | `ValidateLifetime` with a one-minute clock skew in the app; tests use zero skew. |
| Role escalation | User assigns themselves `platform-admin` in JSON | Roles come from the signed token. Unknown roles are dropped. Policies check permissions, not client-supplied role lists. |
| Malicious client-controlled claims/data | Extra fields, spoofed subject, spoofed tenant | Untrusted JSON cannot set `TenantId`. Subject and tenant are taken from validated claims only. |
| Token leakage through logs | Bearer token or password in log sinks | Structured scopes exclude Authorization headers and secrets. `/api/me` never returns the token. |
| Overprivileged platform administrator | Platform role can read every customer | `platform-admin` maps only to `Platform.Manage`. No cross-tenant Customer API. |
| Misconfigured Keycloak client | Confidential secret committed, or Swagger as a privileged client | Swagger is public + PKCE. Audience is a dedicated resource-server client. No production secrets in git. |
| Unprotected administration endpoint | `/api/platform/tenants` left anonymous | `[Authorize(Policy = Platform.Manage)]`. Tests reject tenant-admin and accept platform-admin. |

## Residual risk

- A stolen access token is still a stolen session for its lifetime.
- InMemory isolation is a teaching implementation. Production needs a real database, migrations, and operational Keycloak hardening.
- Realm import must keep organization IDs aligned with seeded `TenantId` values.
