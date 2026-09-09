using Customer.Api.Authentication;
using Customer.Api.Extensions;
using Customer.Api.Logging;
using Customer.Common.Identity;
using Customer.Repository;
using Customer.Repository.Interface;
using Customer.Service;
using Customer.Service.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddCustomerSwagger(builder.Configuration);
builder.Services.AddCustomerAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
builder.Services.AddScoped<ApplicationIdentity>();
builder.Services.AddScoped<IApplicationIdentity>(sp => sp.GetRequiredService<ApplicationIdentity>());
builder.Services.AddSingleton<TenantClaimParser>();
builder.Services.AddSingleton<KeycloakRoleNormalizer>();

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseInMemoryDatabase("CustomerDb"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await CustomerDataSeeder.SeedAsync(dbContext);
}

app.UseCustomerExceptionHandling();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<IdentityResolutionMiddleware>();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.UseCustomerSwagger();
}

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous();

app.Run();

public partial class Program;
