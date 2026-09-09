using Customer.Api.Authorization;
using Customer.Common.Authorization;
using Customer.Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Customer.Api.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddCustomerAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var authentication = configuration
            .GetSection(AuthenticationOptions.SectionName)
            .Get<AuthenticationOptions>() ?? new AuthenticationOptions();

        services.Configure<AuthenticationOptions>(
            configuration.GetSection(AuthenticationOptions.SectionName));

        var requireHttpsMetadata = authentication.RequireHttpsMetadata ?? !environment.IsDevelopment();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.Authority = authentication.Authority;
                options.Audience = authentication.Audience;
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = authentication.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("Customer.Api.Authentication");
                        logger.LogWarning("Authentication failed: {FailureType}", context.Exception.GetType().Name);
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("Customer.Api.Authorization");
                        logger.LogWarning(
                            "Authorization failed for {Method} {Path}",
                            context.Request.Method,
                            context.Request.Path.Value);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(AuthorizationPolicies.CanReadCustomers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.CustomersRead));
                policy.AddRequirements(new TenantContextRequirement());
            });

            options.AddPolicy(AuthorizationPolicies.CanManageCustomers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.CustomersManage));
                policy.AddRequirements(new TenantContextRequirement());
            });

            options.AddPolicy(AuthorizationPolicies.CanDeleteCustomers, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.CustomersDelete));
                policy.AddRequirements(new TenantContextRequirement());
            });

            options.AddPolicy(AuthorizationPolicies.PlatformAdministration, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.PlatformAdminister));
            });
        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, TenantContextAuthorizationHandler>();
        return services;
    }
}
