using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.WebApi.Contracts.Responses.V1.Categories;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.Categories;

public class GetAllEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/categories";
    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet(EndpointUrl, async ([FromServices] IMediator mediator,
            [FromQuery] Guid organizationId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllCategoriesHandler.Query(organizationId);
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(TypeAdapter.Adapt<List<CategoryResponse>>(result));
        })
                .Produces<List<CategoryResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0)
            .RequireAuthorization()
            .WithTags("Categories");
    }
}