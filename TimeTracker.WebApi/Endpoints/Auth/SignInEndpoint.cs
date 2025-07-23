using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Options;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.Auth;
using TimeTracker.WebApi.Extensions;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.Auth
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
                var query = TypeAdapter.Adapt<SignInHandler.Query>(request);
                return await mediator.SendAndProcessResponseAsync<SignInHandler.Query, SignInResponse>(query);
            }).WithTags("Auth");
        }
    }
}