using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Builders.Application.Auth
{
    public class RememberMeSignInQueryBuilder : EntityBuilder<RememberMeSignInQueryBuilder, RememberMeSignInHandler.Query>
    {
        private string _rememberMeToken = Random.Shared.GenerateString(20, 50);

        protected override RememberMeSignInHandler.Query Instance => new RememberMeSignInHandler.Query(_rememberMeToken);

        public RememberMeSignInQueryBuilder WithRememberMeToken(string rememberMeToken)
        {
            _rememberMeToken = rememberMeToken;
            return this;
        }
    }
}