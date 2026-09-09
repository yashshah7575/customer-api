# Local Keycloak

This directory bootstraps a **local-development** Keycloak 26.7.x realm for the Customer API.

`start-dev` and Keycloak's built-in development store are **not production configuration**.

## Start Keycloak

From the repository root:

```bash
docker compose up keycloak
```

Admin console: [http://localhost:8080](http://localhost:8080)

- Realm: `customer-platform`
- Admin user: `admin` / `admin` (local fake credentials only)

The realm file at `import/customer-platform-realm.json` is imported on first start (`--import-realm`).

## After first import: attach users to organizations

Keycloak Organizations membership is not always fully restored from a realm JSON. In the admin console:

1. Open realm `customer-platform`.
2. Confirm **Organizations** is enabled.
3. Confirm organizations exist with these **pinned IDs** (they must match demo seed data):

| Alias | Name | Organization ID used as TenantId |
|---|---|---|
| `acme-bank` | Acme Bank | `11111111-aaaa-4bbb-8ccc-111111111111` |
| `contoso-finance` | Contoso Finance | `22222222-aaaa-4bbb-8ccc-222222222222` |

4. Add members:

- `acme-bank`: `acme.reader`, `acme.editor`, `acme.admin`
- `contoso-finance`: `contoso.reader`, `contoso.editor`, `contoso.admin`
- `platform.admin` has no organization membership

5. On client scope `organization`, open the **Organization Membership** mapper and enable **Add organization ID**.

6. On client `customer-api-swagger`, confirm:

- Public client, no secret
- PKCE S256
- Standard flow enabled
- Default scopes include `openid`, `profile`, `organization`
- Audience mapper includes `customer-api` on the access token

## Demo users

All passwords are fake local-development values: `DevPassword123!`

| Username | Organization | Client role |
|---|---|---|
| `acme.reader` | acme-bank | `tenant-reader` |
| `acme.editor` | acme-bank | `tenant-editor` |
| `acme.admin` | acme-bank | `tenant-admin` |
| `contoso.reader` | contoso-finance | `tenant-reader` |
| `contoso.editor` | contoso-finance | `tenant-editor` |
| `contoso.admin` | contoso-finance | `tenant-admin` |
| `platform.admin` | none | `platform-admin` |

## Swagger login

1. Start Keycloak.
2. Run the API (`dotnet run --project src/Customer.Api`).
3. Open [http://localhost:5080/swagger](http://localhost:5080/swagger).
4. Click **Authorize**.
5. Sign in as a demo user.
6. If the user belongs to more than one organization, Keycloak prompts for a single organization because the client requests the `organization` scope.

## Optional Keycloak checks

The primary test suite does **not** call Keycloak. After Keycloak is running you can inspect a token at [http://localhost:8080/realms/customer-platform/.well-known/openid-configuration](http://localhost:8080/realms/customer-platform/.well-known/openid-configuration).

```bash
dotnet test src/Customer.Api.sln --filter Category=Keycloak
```

Those tests are skipped by default so CI stays deterministic.
