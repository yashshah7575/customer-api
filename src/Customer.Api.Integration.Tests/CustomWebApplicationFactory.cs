using Customer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Customer.Api.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"CustomerTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("Authentication:Authority", TestJwtIssuer.Issuer);
        builder.UseSetting("Authentication:Audience", TestJwtIssuer.Audience);

        builder.ConfigureTestServices(services =>
        {
            var descriptors = services
                .Where(descriptor =>
                    descriptor.ServiceType == typeof(CustomerDbContext) ||
                    descriptor.ServiceType == typeof(DbContextOptions) ||
                    descriptor.ServiceType == typeof(DbContextOptions<CustomerDbContext>))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<CustomerDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = null!;
                options.MetadataAddress = null!;
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.Configuration = null!;
                options.ConfigurationManager = null!;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = TestJwtIssuer.Issuer,
                    ValidAudience = TestJwtIssuer.Audience,
                    IssuerSigningKey = TestJwtIssuer.SigningKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        CustomerDataSeeder.SeedAsync(dbContext).GetAwaiter().GetResult();

        return host;
    }
}
