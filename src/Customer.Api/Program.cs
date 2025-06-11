using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Customer.Repository;
using Customer.Repository.Interface;
using Customer.Service;
using Customer.Service.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 👇 Register Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

//Service
builder.Services.AddScoped<ICustomerService, CustomerService>();

//Repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

//Automapper
//builder.Services.AddAutoMapper(typeof(CustomerMappingProfile));

//DB Registration
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseInMemoryDatabase("CustomerDb"));

//Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();


if (app.Environment.IsDevelopment() || true) // enable always for testing
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello From Yash!");

app.Run();