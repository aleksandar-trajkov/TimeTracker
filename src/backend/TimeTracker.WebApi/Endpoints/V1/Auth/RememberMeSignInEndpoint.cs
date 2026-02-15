using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.V1.Auth;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.V1.Auth;

public class RememberMeSignInEndpoint : IEndpointDefinition
{
    internal static readonly string EndpointUrl = "/auth/rememberme-signin";

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app.MapPost(EndpointUrl, async (
            [FromServices] IMediator mediator,
            [FromBody] RememberMeSignInRequest request, 
            CancellationToken cancellationToken) =>
        {
            var query = request.Adapt<RememberMeSignInHandler.Query>();
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(TypeAdapter.Adapt<SignInResponse>(result));
        })
            .Produces<SignInResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithTags("Auth");
    }
}