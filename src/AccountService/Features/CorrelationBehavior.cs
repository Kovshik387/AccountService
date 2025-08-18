using AccountService.Features.Interfaces;
using MediatR;

namespace AccountService.Features;

public sealed class CorrelationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IHttpContextAccessor _http;

    public CorrelationBehavior(IHttpContextAccessor http) => _http = http;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICorrelationMessage message) return await next(cancellationToken);
        
        message.MessageId = message.MessageId == Guid.Empty ? Guid.NewGuid() : message.MessageId;

        var correlationId = System.Diagnostics.Activity.Current?.TraceId.ToString()
                   ?? _http.HttpContext?.Request.Headers["X-Correlation-Id"].ToString()
                   ?? _http.HttpContext?.TraceIdentifier
                   ?? Guid.NewGuid().ToString();

        message.CorrelationId ??= correlationId;

        return await next(cancellationToken);
    }
}
