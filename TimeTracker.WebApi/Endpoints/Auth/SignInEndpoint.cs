using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.Domain.Options;
using TimeTracker.WebApi.Contracts.Requests.Auth;
using TimeTracker.WebApi.Contracts.Responses.Auth;
using TimeTracker.WebApi.Helpers;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Endpoints.Auth
{
    public class SignInEndpoint : IMinimalApiEndpointDefinition
    {
        public IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
        {
            return app.MapPost("/auth/signin", async (
                [FromServices] IMediator mediator,
                [FromServices] IMapper mapper,
                [FromServices] IOptions<AuthOptions> authOptions,
                [FromBody] SignInRequest request) =>
            {

                var user = await mediator.Send(new SignIn.Query(request.Email, request.Password), CancellationToken.None);

                var jwt = AuthHelper.CreateToken(user, authOptions.Value);
                var rememberMeToken = AuthHelper.CreateRememberMeToken(request.Email, request.RememberMe, authOptions.Value);

                return Results.Ok(new SignInResponse
                {
                    Token = jwt,
                    RememberMeToken = rememberMeToken
                });
            }).WithTags("Auth");
        }
    }
}