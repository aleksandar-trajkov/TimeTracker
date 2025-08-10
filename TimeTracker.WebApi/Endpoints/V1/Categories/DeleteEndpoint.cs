using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.Categories;

public class DeleteEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/categories/{id:guid}";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapDelete(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromRoute] Guid id) =>
        {
            var command = new DeleteCategoryHandler.Command(id);
            return await mediator.SendAsync<DeleteCategoryHandler.Command>(command);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("Categories");
    }
}