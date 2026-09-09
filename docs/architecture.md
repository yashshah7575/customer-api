# Architecture

The Customer API is a B2B resource server. The interesting architecture is identity, authorization, and tenant isolation—not the Customer CRUD itself.

See [identity-architecture.md](identity-architecture.md) and [security-design.md](security-design.md) for the staff-level boundary between Keycloak and the API.

## Layers

```
HTTP / Swagger
  -> Authentication (JWT Bearer + OIDC discovery)
  -> Identity resolution (tenant_id + roles)
  -> Authorization policies
  -> Controllers
  -> CustomerService
  -> CustomerRepository
  -> CustomerDbContext (global tenant filter)
```

Keycloak-specific JSON stays in `Customer.Api`. `Customer.Common` exposes `ITenantContext` and permission names. Repository and service code depend on those abstractions only.

## Hosting

Run the API locally against Dockerized Keycloak. AWS Lambda and Terraform files under `src/` are leftovers from an earlier iteration and are not required for this sample.

## Persistence

`CustomerDbContext` uses EF Core InMemory so the repository can be cloned and run without a database product. That choice is a demo constraint, not a persistence recommendation.

## Request correlation

`RequestLoggingMiddleware` writes `CorrelationId`, `SubjectId`, `TenantId`, `IsPlatformAdmin`, method, route, and status. It never logs bearer tokens or Authorization headers.
