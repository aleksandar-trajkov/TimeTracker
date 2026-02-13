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
            [FromQuery] Guid organizationId) =>
        {
            var result = await mediator.Send(new GetAllCategoriesHandler.Query(organizationId));
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