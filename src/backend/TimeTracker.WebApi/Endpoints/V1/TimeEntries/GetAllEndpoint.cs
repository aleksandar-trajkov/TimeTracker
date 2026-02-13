using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.WebApi.Contracts.Responses.V1.TimeEntries;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.TimeEntries;

public class GetAllEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/time-entries";
    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet(EndpointUrl, async ([FromServices] IMediator mediator,
            [FromQuery] DateTimeOffset from,
            [FromQuery] DateTimeOffset to) =>
        {
            var result = await mediator.Send(new GetAllTimeEntriesHandler.Query(from, to));
            return Results.Ok(TypeAdapter.Adapt<List<TimeEntryResponse>>(result));
        })
                .Produces<List<TimeEntryResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0)
            .RequireAuthorization()
            .WithTags("TimeEntries");
    }
}