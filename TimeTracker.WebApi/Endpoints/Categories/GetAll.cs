using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using TimeTracker.Domain.Options;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.Categories;

public class GetAll : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/api/categories";
    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapGet(EndpointUrl, async () =>
        {
            // This is a placeholder for the actual implementation.
            // You would typically retrieve user information or perform authentication here.
            return Results.Ok("This is a placeholder response for the GetAll endpoint.");
        })
            .RequireAuthorization()
            .WithTags("Categories");
    }
}