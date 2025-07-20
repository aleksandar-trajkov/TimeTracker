using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.Auth;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.Auth
{
    public class RememberMeSignInEndpoint : IEndpointDefinition
    {
        internal static readonly string EndpointUrl = "/api/auth/rememberme-signin";
        
        public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app.MapPost(EndpointUrl, async (
                [FromServices] IMediator mediator,
                [FromServices] ITokenProvider tokenProvider,
                [FromBody] RememberMeSignInRequest request) =>
            {
                var query = TypeAdapter.Adapt<RememberMeSignInHandler.Query>(request);
                return await mediator.SendAndProcessResponseAsync<RememberMeSignInHandler.Query, SignInResponse>(query);
            }).WithTags("Auth");
        }
    }
}