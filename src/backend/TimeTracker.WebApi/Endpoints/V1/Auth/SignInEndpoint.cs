using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.V1.Auth;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.Auth
{
    public class SignInEndpoint : IEndpointDefinition
    {
        internal static readonly string EndpointUrl = "/auth/signin";
        public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app.MapPost(EndpointUrl, async (
                [FromServices] IMediator mediator,
                [FromBody] SignInRequest request) =>
            {
                var query = request.Adapt<SignInHandler.Query>();
                var result = await mediator.Send(query);
                return Results.Ok(TypeAdapter.Adapt<SignInResponse>(result));
            })
                .Produces<SignInResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .HasApiVersion(1.0)
                .WithTags("Auth");
        }
    }
}