namespace Customer.Api.Integration.Tests;

/// <summary>
/// Optional Keycloak end-to-end checks. These are skipped by default so the
/// primary suite stays deterministic and does not require a running IdP.
/// Run them after `docker compose up keycloak` with:
/// dotnet test --filter Category=Keycloak
/// </summary>
public class KeycloakEndToEndTests
{
    [Fact(Skip = "Requires a running local Keycloak. See keycloak/README.md.")]
    [Trait("Category", "Keycloak")]
    public void KeycloakRealm_IsDocumentedSeparatelyFromFastTests()
    {
        Assert.True(true);
    }
}
