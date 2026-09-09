# Keycloak (local development / demonstration only)

This folder is **not** a production identity platform.

`docker compose up -d` starts Keycloak 26 and imports `realm-export.json` into realm `customer-api-demo`.

Demo passwords and the machine-client secret in the realm file are throwaway local values. Do not reuse them outside this repository.
