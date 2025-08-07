using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AccountService.Features;
using AccountService.Features.Exceptions;
using FluentValidation;

namespace AccountService.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        object errorResponse;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                
                var validationErrors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).First()
                    );
                errorResponse = MbResult.Error("Validation error", validationErrors); 
                break;

            case AccountNotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                errorResponse = MbResult.Error("Not found", GetErrorMessage(notFoundEx.Message));
                break;        
            
            case TransactionNotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                errorResponse = MbResult.Error("Not found", GetErrorMessage(notFoundEx.Message));
                break;

            case AccountException accountEx:
                statusCode = StatusCodes.Status400BadRequest;
                errorResponse = MbResult.Error("Account error", GetErrorMessage(accountEx.Message));
                break;

            default:
                _logger.LogError(exception, "Unhandled exception");
                statusCode = StatusCodes.Status500InternalServerError;
                errorResponse = MbResult.Error("Internal server error", GetErrorMessage(exception.Message));
                break;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private static Dictionary<string, string> GetErrorMessage(string error) => new()
    {
        { "message", error }
    };
}
