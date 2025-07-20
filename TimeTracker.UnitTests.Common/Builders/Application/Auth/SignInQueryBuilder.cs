using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Builders.Application.Auth
{
    public class SignInQueryBuilder : EntityBuilder<SignInQueryBuilder, SignInRequestHandler.Query>
    {
        private string _email = Random.Shared.GenerateEmail(10);
        private string _password = Random.Shared.GenerateString(10, 30);
        private bool _rememberMe = true;

        protected override SignInRequestHandler.Query Instance => new SignInRequestHandler.Query(_email, _password, _rememberMe);

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
