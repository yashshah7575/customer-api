using Customer.Common.Identity;

namespace Customer.Api.Logging;

public sealed class RequestLoggingMiddleware
{
    public const string CorrelationHeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var correlationId = context.Request.Headers[CorrelationHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = context.TraceIdentifier;
        }

        context.Response.Headers[CorrelationHeaderName] = correlationId;

        var route = context.Request.Path.Value ?? string.Empty;

        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["SubjectId"] = tenantContext.SubjectId,
            ["TenantId"] = tenantContext.TenantId,
            ["IsPlatformAdmin"] = tenantContext.IsPlatformAdmin,
            ["HttpMethod"] = context.Request.Method,
            ["Route"] = route
        }))
        {
            await _next(context);

            _logger.LogInformation(
                "Handled {HttpMethod} {Route} with status {StatusCode}",
                context.Request.Method,
                route,
                context.Response.StatusCode);
        }
    }
}
