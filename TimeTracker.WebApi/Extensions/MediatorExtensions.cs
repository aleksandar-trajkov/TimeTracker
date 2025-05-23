using TimeTracker.Application.Behaviours;
using TimeTracker.WebApi.Contracts.Responses;
using Azure.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Mapster;
using TimeTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace TimeTracker.WebApi.Extensions;

public static class MediatorExtensions
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
                return Results.NotFound(MapErrors(validationEx));
            }
            else if(validationEx.Errors.All(x => x.ErrorCode == ValidationErrorCodes.Conflict))
            {
                return Results.Conflict(MapErrors(validationEx));
            }
            else
            {
                return Results.BadRequest(MapErrors(validationEx));
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

    private static IEnumerable<ValidationErrorResponse> MapErrors(ValidationException validationEx)
    {
        return validationEx.Errors.Select(x => new ValidationErrorResponse
        {
            Property = x.PropertyName,
            Message = x.ErrorMessage
        });
    }
}
