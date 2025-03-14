using Microsoft.Extensions.Primitives;
using Catalog.Core.SharedKernel.Correlation;

namespace Catalog.Api.Hosted.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderKey = "X-Correlation-Id";
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        var correlationId = GetCorrelationId(context, correlationIdGenerator);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(CorrelationIdHeaderKey, new[] { correlationId.ToString() });
            return Task.CompletedTask;
        });

        await _next(context);
    }

    private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationId))
        {
            correlationIdGenerator.Set(correlationId);
            return correlationId;
        }

        return correlationIdGenerator.Get();
    }
}