# Threat model (demo scope)

| Threat | Attack | Mitigation |
|---|---|---|
| Forged TenantId | Caller puts another tenant in a header, route, or body | Tenant comes only from a validated `tenant_id` claim. Body `tenantId` is ignored. Create uses `ITenantContext`. |
| Missing tenant | Token has roles but no `tenant_id` | Non-platform callers are denied tenant-scoped routes. |
| Cross-tenant IDOR | Guess another tenant's GUID | Global query filter + 404. Repositories do not use `Find`/`FindAsync`. |
| Role escalation | User assigns themselves `PlatformAdmin` in JSON | Roles come from the signed token. Unknown roles are dropped. Policies check permissions. |
| Overprivileged machine client | Integration client is a realm admin | `customer-a-integration` is `ServiceClient` only, tenant-bound, read-only. |
| Unprotected administration endpoint | `/api/platform/tenants` left anonymous | `[Authorize(Policy = PlatformAdministration)]`. |
| Token replay after expiry | Old JWT reused | Lifetime validation is enabled. |
| Unsigned / foreign token | Wrong issuer, audience, or key | JWT Bearer validates issuer, audience, signature, and lifetime. |

Demo passwords and the local machine-client secret are throwaway values. They must never be used as production recommendations.
