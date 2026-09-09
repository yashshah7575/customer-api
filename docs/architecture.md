# Architecture

## Purpose

The Customer API is a B2B resource server. The interesting architecture is identity, authorization, and tenant isolation—not the Customer CRUD itself.

## Layers

```
HTTP / Swagger
  -> Authentication (JWT Bearer + OIDC discovery)
  -> Identity resolution (organization + roles)
  -> Authorization policies
  -> Controllers
  -> CustomerService
  -> CustomerRepository
  -> CustomerDbContext (global tenant filter)
```

Keycloak-specific JSON stays in `Customer.Api`. `Customer.Common` exposes `ITenantContext` and permission names. Repository and service code depend on those abstractions only.

## Why this split

Identity work stays useful when these stay separate:

1. **Proof of identity** — token validation by the framework
2. **Meaning of identity** — parsers that turn claims into tenant and permission sets
3. **Enforcement** — policies and query filters that cannot be bypassed by forgetting a `Where`

## Hosting

`Program.cs` uses the ASP.NET Core minimal host. Run the API locally against Dockerized Keycloak. AWS Lambda and Terraform files under `src/` are leftovers from an earlier iteration and are not required for this sample.

## Persistence

`CustomerDbContext` uses EF Core InMemory so the repository can be cloned and run without a database product. That choice is documented because it is a demo constraint, not a persistence recommendation.

## Request correlation

`RequestLoggingMiddleware` writes `CorrelationId`, `SubjectId`, `TenantId`, `TenantAlias`, method, route, and status. It never logs bearer tokens or Authorization headers.
