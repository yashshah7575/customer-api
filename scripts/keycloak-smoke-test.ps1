# Smoke-tests a REAL Keycloak-issued Client Credentials token against the API.
# LOCAL DEVELOPMENT / DEMONSTRATION ONLY. Not part of `dotnet test`.
$ErrorActionPreference = "Stop"

$rootDir = Split-Path -Parent $PSScriptRoot
$keycloakUrl = if ($env:KEYCLOAK_URL) { $env:KEYCLOAK_URL } else { "http://localhost:8080" }
$realm = if ($env:REALM) { $env:REALM } else { "customer-api-demo" }
$apiUrl = if ($env:API_URL) { $env:API_URL } else { "http://localhost:5080" }
$clientId = if ($env:CLIENT_ID) { $env:CLIENT_ID } elseif ($env:CUSTOMER_A_INTEGRATION_CLIENT_ID) { $env:CUSTOMER_A_INTEGRATION_CLIENT_ID } else { "customer-a-integration" }
$clientSecret = if ($env:CLIENT_SECRET) { $env:CLIENT_SECRET } else { $env:CUSTOMER_A_INTEGRATION_SECRET }

if (-not $clientSecret -and (Test-Path (Join-Path $rootDir ".env"))) {
    Get-Content (Join-Path $rootDir ".env") | ForEach-Object {
        if ($_ -match "^\s*#" -or $_ -notmatch "=") { return }
        $name, $value = $_.Split("=", 2)
        Set-Item -Path "Env:$name" -Value $value
    }
    if (-not $clientSecret) {
        $clientSecret = $env:CUSTOMER_A_INTEGRATION_SECRET
    }
}

if (-not $clientSecret) {
    throw "Set CLIENT_SECRET or CUSTOMER_A_INTEGRATION_SECRET (see .env.example)."
}

$tokenUrl = "$keycloakUrl/realms/$realm/protocol/openid-connect/token"
$wellKnown = "$keycloakUrl/realms/$realm/.well-known/openid-configuration"

Write-Host "Waiting for Keycloak at $wellKnown"
$ready = $false
for ($i = 0; $i -lt 60; $i++) {
    try {
        Invoke-WebRequest -Uri $wellKnown -UseBasicParsing | Out-Null
        $ready = $true
        break
    } catch {
        Start-Sleep -Seconds 2
    }
}

if (-not $ready) {
    throw "Keycloak is not reachable. Start it with: docker compose up -d"
}

Write-Host "Requesting access token for client $clientId"
$tokenResponse = Invoke-RestMethod -Method Post -Uri $tokenUrl -ContentType "application/x-www-form-urlencoded" -Body @{
    grant_type    = "client_credentials"
    client_id     = $clientId
    client_secret = $clientSecret
}

if (-not $tokenResponse.access_token) {
    throw "Keycloak did not return access_token"
}

Write-Host "Calling $apiUrl/api/customers"
$response = Invoke-WebRequest -Uri "$apiUrl/api/customers" -Headers @{
    Authorization = "Bearer $($tokenResponse.access_token)"
}

if ([int]$response.StatusCode -ne 200) {
    throw "Expected HTTP 200 from /api/customers, received $($response.StatusCode)"
}

Write-Host "Smoke test passed: Keycloak token was accepted and Customer A data was returned."
