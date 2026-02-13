using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.WebApi.Contracts.Responses.V1.TimeEntries;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.TimeEntries;

public class GetByIdEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/time-entries/{id:guid}";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromRoute] Guid id) =>
        {
            var result = await mediator.Send(new GetTimeEntryByIdHandler.Query(id));
            return Results.Ok(TypeAdapter.Adapt<TimeEntryResponse>(result));
        })
        .Produces<TimeEntryResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("TimeEntries");
    }
}