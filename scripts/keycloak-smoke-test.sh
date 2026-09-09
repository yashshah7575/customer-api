#!/usr/bin/env bash
# Smoke-tests a REAL Keycloak-issued Client Credentials token against the API.
# LOCAL DEVELOPMENT / DEMONSTRATION ONLY. Not part of `dotnet test`.
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
KEYCLOAK_URL="${KEYCLOAK_URL:-http://localhost:8080}"
REALM="${REALM:-customer-api-demo}"
API_URL="${API_URL:-http://localhost:5080}"
CLIENT_ID="${CLIENT_ID:-${CUSTOMER_A_INTEGRATION_CLIENT_ID:-customer-a-integration}}"
CLIENT_SECRET="${CLIENT_SECRET:-${CUSTOMER_A_INTEGRATION_SECRET:-}}"

if [[ -z "${CLIENT_SECRET}" && -f "${ROOT_DIR}/.env" ]]; then
  # shellcheck disable=SC1091
  set -a
  source "${ROOT_DIR}/.env"
  set +a
  CLIENT_SECRET="${CLIENT_SECRET:-${CUSTOMER_A_INTEGRATION_SECRET:-}}"
fi

if [[ -z "${CLIENT_SECRET}" ]]; then
  echo "Set CLIENT_SECRET or CUSTOMER_A_INTEGRATION_SECRET (see .env.example)." >&2
  exit 1
fi

TOKEN_URL="${KEYCLOAK_URL}/realms/${REALM}/protocol/openid-connect/token"
WELL_KNOWN="${KEYCLOAK_URL}/realms/${REALM}/.well-known/openid-configuration"

echo "Waiting for Keycloak at ${WELL_KNOWN}"
for _ in $(seq 1 60); do
  if curl -fsS "${WELL_KNOWN}" >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

if ! curl -fsS "${WELL_KNOWN}" >/dev/null 2>&1; then
  echo "Keycloak is not reachable. Start it with: docker compose up -d" >&2
  exit 1
fi

echo "Requesting access token for client ${CLIENT_ID}"
TOKEN_RESPONSE="$(curl -fsS -X POST "${TOKEN_URL}" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  --data-urlencode "grant_type=client_credentials" \
  --data-urlencode "client_id=${CLIENT_ID}" \
  --data-urlencode "client_secret=${CLIENT_SECRET}")"

TOKEN="$(printf '%s' "${TOKEN_RESPONSE}" | python3 -c 'import json,sys; token=json.load(sys.stdin).get("access_token");
assert token, "Keycloak did not return access_token"; print(token)')"

if [[ -z "${TOKEN}" ]]; then
  echo "Failed to parse access_token from Keycloak." >&2
  exit 1
fi

echo "Calling ${API_URL}/api/customers"
HTTP_CODE="$(curl -sS -o /tmp/customer-api-smoke-body.json -w "%{http_code}" \
  -H "Authorization: Bearer ${TOKEN}" \
  "${API_URL}/api/customers")"

if [[ "${HTTP_CODE}" != "200" ]]; then
  echo "Expected HTTP 200 from /api/customers, received ${HTTP_CODE}" >&2
  cat /tmp/customer-api-smoke-body.json >&2 || true
  exit 1
fi

echo "Smoke test passed: Keycloak token was accepted and Customer A data was returned."
