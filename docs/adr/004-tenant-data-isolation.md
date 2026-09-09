# 004. Tenant data isolation

## Context

Customer rows from Acme Bank and Contoso Finance share one application database (InMemory in this demo). A missed `Where(c => c.TenantId == current)` is an IDOR.

EF Core `Find` / `FindAsync` load by primary key and **bypass** global query filters. That is a silent cross-tenant leak if a GUID is guessed or leaked.

## Decision

1. Require `TenantId` on `CustomerEntity`.
2. Apply a global query filter: current tenant only.
3. Set `TenantId` on create from `ITenantContext`, never from the client.
4. Query by id with LINQ so the filter applies.
5. Return 404 for cross-tenant ids.
6. Keep platform operations on separate endpoints that do not query tenant customers.

## Alternatives considered

**Filter only in repositories.** Works until someone adds a new query. Filters plus repository discipline is defense in depth.

**Separate database per tenant.** Strong isolation, high operational cost, out of scope for this reference.

**Row-level security in the database.** The right production complement. This demo has no production database.

## Consequences

- Developers cannot "just Find by id" without breaking isolation; the repository does not expose `Find`.
- Tests prove Acme cannot read Contoso by id.
- InMemory demonstrates the pattern; production should add a real database and still keep the filter.
