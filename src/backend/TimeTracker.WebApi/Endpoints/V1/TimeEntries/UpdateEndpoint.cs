using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.WebApi.Contracts.Requests.V1.TimeEntries;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.TimeEntries;

public class UpdateEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/time-entries/{id:guid}";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPut(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromRoute] Guid id,
            [FromBody] UpdateTimeEntryRequest request) =>
        {
            var command = new UpdateTimeEntryHandler.Command(id, request.From, request.To, request.Description, request.CategoryId);
            var result = await mediator.Send(command);
            return Results.Ok(TypeAdapter.Adapt<bool>(result));
        })
        .Produces<bool>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("TimeEntries");
    }
}