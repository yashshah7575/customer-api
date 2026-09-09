# Multi-tenancy

Demo tenants:

| Tenant | `tenant_id` |
|---|---|
| Customer A | `customer-a` |
| Customer B | `customer-b` |

The internal tenant key is the `tenant_id` claim on a validated access token. The API never treats a request body, query string, header, or route value as proof of membership.

Non-platform users must have exactly one `tenant_id`. Missing or conflicting claims fail closed (403) on tenant-scoped routes.

`PlatformAdmin` may omit `tenant_id` and read across tenants. Creates still require a tenant so rows always have an owner.

Seeded customers:

- Customer A: Alice Nguyen, Aaron Patel
- Customer B: Bob Diaz, Bella Okoye

Logging in as Customer A never returns Customer B rows. Requesting a Customer B id as Customer A returns 404.
