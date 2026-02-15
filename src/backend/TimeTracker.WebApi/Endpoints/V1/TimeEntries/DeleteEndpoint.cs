using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.TimeEntries;

public class DeleteEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/time-entries/{id:guid}";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapDelete(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromRoute] Guid id,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteTimeEntryHandler.Command(id);
            await mediator.Send(command, cancellationToken);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("TimeEntries");
    }
}