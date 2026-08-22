using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Inventory_Management_dotnet.Exceptions;
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        _logger.LogError(exception, "An unhandled exception occured: {Message}", exception.Message);

        // Map Exceptions' titles and status codes
        var (statusCode, title) = exception switch
        {
            AppException appEx => ((int)appEx.statusCode, ReasonPhrases.GetReasonPhrase((int)appEx.statusCode)),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError ? "Internal Server Error" : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        if(exception is ValidationException validationEx)
        {
            problemDetails.Extensions["errors"] = validationEx.Errors;
        }

        // Write the JSON response
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // return true to indicate that pipeline stops here
        return true;
    }
}