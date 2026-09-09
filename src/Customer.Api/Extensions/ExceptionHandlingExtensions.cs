using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Extensions;

public static class ExceptionHandlingExtensions
{
    public static WebApplication UseCustomerExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = feature?.Error;
                var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

                var (statusCode, title) = exception switch
                {
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                    InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid request"),
                    _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15",
                    Detail = environment.IsDevelopment() ? exception?.Message : null
                };

                await context.Response.WriteAsJsonAsync(problem);
            });
        });

        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            if (response.HasStarted || response.ContentType?.Contains("problem+json") == true)
            {
                return;
            }

            var problem = new ProblemDetails
            {
                Status = response.StatusCode,
                Title = response.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status403Forbidden => "Forbidden",
                    StatusCodes.Status404NotFound => "Resource not found",
                    _ => "Request failed"
                },
                Type = "https://tools.ietf.org/html/rfc9110#section-15"
            };

            response.ContentType = "application/problem+json";
            await response.WriteAsJsonAsync(problem);
        });

        return app;
    }
}
