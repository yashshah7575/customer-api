using Customer.Common.Options;
using Microsoft.OpenApi.Models;

namespace Customer.Api.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddCustomerSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authentication = configuration
            .GetSection(AuthenticationOptions.SectionName)
            .Get<AuthenticationOptions>() ?? new AuthenticationOptions();

        var authority = authentication.Authority.TrimEnd('/');

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Customer API",
                Version = "v1",
                Description = "Multi-tenant Customer API secured with OAuth 2.0, OpenID Connect, and Keycloak Organizations."
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{authority}/protocol/openid-connect/auth"),
                        TokenUrl = new Uri($"{authority}/protocol/openid-connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID Connect",
                            ["profile"] = "Profile",
                            ["organization"] = "Keycloak organization membership"
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new[] { "openid", "profile", "organization" }
                }
            });
        });

        return services;
    }

    public static WebApplication UseCustomerSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API v1");
            options.OAuthClientId("customer-api-swagger");
            options.OAuthUsePkce();
            options.OAuthScopeSeparator(" ");
            options.OAuthScopes("openid", "profile", "organization");
            options.OAuthAppName("Customer API Swagger");
        });

        return app;
    }
}
