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

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.Authority = authentication.Authority;
                options.Audience = authentication.Audience;
                options.RequireHttpsMetadata = !environment.IsDevelopment();
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
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.CustomersRead, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.CustomersRead));
                policy.AddRequirements(new TenantContextRequirement());
            });

            options.AddPolicy(AuthorizationPolicies.CustomersWrite, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.CustomersWrite));
                policy.AddRequirements(new TenantContextRequirement());
            });

            options.AddPolicy(AuthorizationPolicies.TenantManage, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.TenantManage));
                policy.AddRequirements(new TenantContextRequirement());
            });

            options.AddPolicy(AuthorizationPolicies.PlatformManage, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new PermissionRequirement(ApplicationPermissions.PlatformManage));
            });
        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, TenantContextAuthorizationHandler>();
        return services;
    }
}
