using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain.Options;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.TimeEntries;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.Categories;

public class GetAllEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/api/categories";
    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet(EndpointUrl, async ([FromServices] IMediator mediator) =>
        {
            return await mediator.SendAndProcessResponseAsync< GetAllCategoriesHandler.Query, List<CategoriesResponse>>(new GetAllCategoriesHandler.Query());
        })
            .RequireAuthorization()
            .WithTags("Categories");
    }
}