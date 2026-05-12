using Marketplace.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.TraceIdentifier;
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title) = exception switch
        {
            ValidationAppException => (StatusCodes.Status400BadRequest, "Validation error"),
            NotFoundAppException => (StatusCodes.Status404NotFound, "Resource not found"),
            ExternalServiceAppException => (StatusCodes.Status502BadGateway, "External service error"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        _logger.LogError(exception, "Request failed. CorrelationId: {CorrelationId}", correlationId);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message,
            Instance = context.Request.Path
        };
        problem.Extensions["correlationId"] = correlationId;

        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(problem);
    }
}
