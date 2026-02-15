using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.WebApi.Contracts.Requests.V1.TimeEntries;
using TimeTracker.WebApi.Contracts.Responses.V1.TimeEntries;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.TimeEntries;

public class CreateEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/time-entries";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromBody] CreateTimeEntryRequest request,
            CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<CreateTimeEntryHandler.Command>();
            var response = await mediator.Send(command, cancellationToken);
            return Results.Ok(TypeAdapter.Adapt<TimeEntryResponse>(response));
        })
        .Produces<TimeEntryResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("TimeEntries");
    }
}