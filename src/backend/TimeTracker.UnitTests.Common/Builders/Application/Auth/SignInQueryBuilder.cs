using TimeTracker.Application.UseCases.Auth.Handlers;
using Bogus;

namespace TimeTracker.UnitTests.Common.Builders.Application.Auth
{
    public class SignInQueryBuilder : EntityBuilder<SignInQueryBuilder, SignInHandler.Query>
    {
        private static readonly Faker Faker = new();

        private string _email = Faker.Internet.Email();
        private string _password = Faker.Random.AlphaNumeric(Faker.Random.Int(10, 30));
        private bool _rememberMe = true;

        protected override SignInHandler.Query Instance => new SignInHandler.Query(_email, _password, _rememberMe);

        public SignInQueryBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public SignInQueryBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public SignInQueryBuilder WithRememberMe(bool rememberMe)
        {
            _rememberMe = rememberMe;
            return this;
        }
    }
}
