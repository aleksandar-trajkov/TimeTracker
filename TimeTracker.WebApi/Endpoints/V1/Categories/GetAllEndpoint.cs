using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain.Options;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.V1.Auth;
using TimeTracker.WebApi.Contracts.Responses.V1.TimeEntries;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Helpers;
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
            return await mediator.SendAndProcessResponseAsync< GetAllCategoriesHandler.Query, List<CategoriesResponse>>(new GetAllCategoriesHandler.Query(organizationId));
        })
                .Produces<List<CategoriesResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1.0)
            .RequireAuthorization()
            .WithTags("Categories");
    }
}