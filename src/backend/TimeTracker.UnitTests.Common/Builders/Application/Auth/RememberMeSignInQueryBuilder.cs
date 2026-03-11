using TimeTracker.Application.UseCases.Auth.Handlers;
using Bogus;

namespace TimeTracker.UnitTests.Common.Builders.Application.Auth
{
    public class RememberMeSignInQueryBuilder : EntityBuilder<RememberMeSignInQueryBuilder, RememberMeSignInHandler.Query>
    {
        private static readonly Faker Faker = new();

        private string _rememberMeToken = Faker.Random.AlphaNumeric(Faker.Random.Int(20, 50));

        protected override RememberMeSignInHandler.Query Instance => new RememberMeSignInHandler.Query(_rememberMeToken);

        public RememberMeSignInQueryBuilder WithRememberMeToken(string rememberMeToken)
        {
            _rememberMeToken = rememberMeToken;
            return this;
        }
    }
}