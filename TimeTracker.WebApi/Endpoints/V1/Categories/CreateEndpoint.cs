using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.WebApi.Contracts.Requests.V1.Categories;
using TimeTracker.WebApi.Contracts.Responses.V1.Categories;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.Categories;

public class CreateEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/categories";
    
    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromBody] CreateCategoryRequest request) =>
        {
            var command = request.Adapt<CreateCategoryHandler.Command>();
            return await mediator.SendAndProcessResponseAsync<CreateCategoryHandler.Command, CreateCategoryResponse>(command);
        })
        .Produces<CreateCategoryResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .MapToApiVersion(1.0)
        .RequireAuthorization()
        .WithTags("Categories");
    }
}