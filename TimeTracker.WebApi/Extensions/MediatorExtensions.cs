using Azure.Core;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TimeTracker.Application.Behaviours;
using TimeTracker.Domain.Exceptions;
using TimeTracker.WebApi.Contracts.Responses;

namespace TimeTracker.WebApi.Extensions;

public static partial class MediatorExtensions
{
    public static async Task<IResult> SendAndProcessResponseAsync<TRequest, TResponse>(this IMediator mediator, TRequest request)
    {
        return await ProcessSendAsync(async () =>
        {
            var result = await mediator.Send(request!);
            return Results.Ok(TypeAdapter.Adapt<TResponse>(result));
        }, request);
    }

    public static async Task<IResult> SendAsync<TRequest>(this IMediator mediator, TRequest request)
    {
        return await ProcessSendAsync(async () =>
        {
            await mediator.Send(request!);
            return Results.Ok();
        }, request);
    }

    public static async Task<IResult> SendAndProcessResponseManuallyAsync<TRequest, TResult, TResponse>(this IMediator mediator, TRequest request, Func<TResult?, Task<TResponse>> processFunc)
        where TResult : class
    {
        return await ProcessSendAsync(async () =>
        {
            var result = await mediator.Send(request!);
            var mappedResult = processFunc(result as TResult);
            return Results.Ok(mappedResult);
        }, request);
    }

    private static async Task<IResult> ProcessSendAsync<TRequest>(Func<Task<IResult>> sendFunc, TRequest request)
    {
        try
        {
            if (request != null)
            {
                return await sendFunc();
            }
            return Results.Problem($"Sent null request of type {typeof(TRequest).Name}");
        }
        catch (ValidationException validationEx)
        {
            if (validationEx.Errors.All(x => x.ErrorCode == ValidationErrorCodes.NotFound))
            {
                return Results.Problem(new ProblemDetails
                {
                    Title = "Not found",
                    Status = StatusCodes.Status404NotFound,
                    Extensions = { ["errors"] = validationEx.ToValidationErrorResponses() }
                });
            }
            else if(validationEx.Errors.All(x => x.ErrorCode == ValidationErrorCodes.Conflict))
            {
                return Results.Problem(new ProblemDetails
                {
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Extensions = { ["errors"] = validationEx.ToValidationErrorResponses() }
                });
            }
            else
            {
                return Results.Problem(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["errors"] = validationEx.ToValidationErrorResponses() }
                });
            }
        }
        catch (AuthenticationException authEx)
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = authEx.Message,
                Status = StatusCodes.Status401Unauthorized,
                Extensions = { ["email"] = authEx.Email }
            });
        }
        catch (AuthorizationException authEx)
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Forbidden",
                Detail = authEx.Message,
                Status = StatusCodes.Status403Forbidden,
                Extensions = { ["email"] = authEx.Email, ["permission"] = authEx.Permission }
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static IEnumerable<ValidationErrorResponse> ToValidationErrorResponses(this ValidationException validationException)
    {
        return validationException.Errors.Select(error => new ValidationErrorResponse
        {
            Property = error.PropertyName,
            Message = error.ErrorMessage
        });
    }
}
