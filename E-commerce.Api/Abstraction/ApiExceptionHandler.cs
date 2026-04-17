using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Api.Abstraction;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {message}", exception.Message);

        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "ValidationError",
                "One or more validation errors occurred.",
                validationException.Errors.Select(x => new { Code = x.PropertyName, Message = x.ErrorMessage }).ToArray()),
            _ => (
                StatusCodes.Status500InternalServerError,
                "InternalServerError",
                "An unexpected server error occurred.",
                Array.Empty<object>())
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };
        problemDetails.Extensions["errors"] = errors;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
