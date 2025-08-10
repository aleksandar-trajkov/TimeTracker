using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.WebApi.Contracts.Requests.V1.Categories;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.Categories;

public class UpdateEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/categories/{id:guid}";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPut(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromRoute] Guid id,
            [FromBody] UpdateCategoryRequest request) =>
        {
            var command = new UpdateCategoryHandler.Command(id, request.Name, request.Description);
            var result = await mediator.Send(command);

            return result ? Results.NoContent() : Results.NotFound();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("Categories");
    }
}