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
using TimeTracker.WebApi.Helpers;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.Auth
{
    public class SignInEndpoint : IMinimalApiEndpointDefinition
    {
        internal static readonly string EndpointUrl = "/auth/signin";
        public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app.MapPost(EndpointUrl, async (
                [FromServices] IMediator mediator,
                [FromServices] IMapper mapper,
                [FromServices] IOptions<AuthOptions> authOptions,
                [FromBody] SignInRequest request) =>
            {
                var query = new SignIn.Query(request.Email, request.Password);
                return await mediator.SendAndProcessResponseManuallyAsync<SignIn.Query, User, SignInResponse>(query, user =>
                {
                    var jwt = AuthHelper.CreateToken(user, authOptions.Value);
                    var rememberMeToken = AuthHelper.CreateRememberMeToken(request.Email, request.RememberMe, authOptions.Value);

                    return Task.FromResult(new SignInResponse
                    {
                        Token = jwt,
                        RememberMeToken = rememberMeToken
                    });
                });
            }).WithTags("Auth");
        }
    }
}