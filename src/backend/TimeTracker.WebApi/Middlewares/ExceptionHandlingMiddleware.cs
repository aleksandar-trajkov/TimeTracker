using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.Behaviours;
using TimeTracker.Domain.Exceptions;
using TimeTracker.WebApi.Contracts.Responses;

namespace TimeTracker.WebApi.Middlewares;

public class ExceptionHandlingMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ValidationException validationEx:
                await HandleValidationException(httpContext, validationEx, cancellationToken);
                return true;

            case AuthenticationException authEx:
                await Results.Problem(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = authEx.Message,
                    Status = StatusCodes.Status401Unauthorized,
                    Extensions = { ["email"] = authEx.Email }
                }).ExecuteAsync(httpContext);
                return true;

            case AuthorizationException authzEx:
                await Results.Problem(new ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = authzEx.Message,
                    Status = StatusCodes.Status403Forbidden,
                    Extensions = { ["email"] = authzEx.Email, ["permission"] = authzEx.Permission }
                }).ExecuteAsync(httpContext);
                return true;

            default:
                _logger.LogError(exception, "An unhandled exception occurred");
                await Results.Problem(exception.Message).ExecuteAsync(httpContext);
                return true;
        }
    }

    private static async Task HandleValidationException(HttpContext httpContext, ValidationException validationEx, CancellationToken cancellationToken)
    {
        var errors = validationEx.Errors.Select(error => new ValidationErrorResponse
        {
            Property = error.PropertyName,
            Message = error.ErrorMessage
        });

        ProblemDetails problemDetails;

        if (validationEx.Errors.All(x => x.ErrorCode == ValidationErrorCodes.NotFound))
        {
            problemDetails = new ProblemDetails
            {
                Title = "Not found",
                Status = StatusCodes.Status404NotFound,
                Extensions = { ["errors"] = errors }
            };
        }
        else if (validationEx.Errors.All(x => x.ErrorCode == ValidationErrorCodes.Conflict))
        {
            problemDetails = new ProblemDetails
            {
                Title = "Conflict",
                Status = StatusCodes.Status409Conflict,
                Extensions = { ["errors"] = errors }
            };
        }
        else
        {
            problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { ["errors"] = errors }
            };
        }

        await Results.Problem(problemDetails).ExecuteAsync(httpContext);
    }
}
