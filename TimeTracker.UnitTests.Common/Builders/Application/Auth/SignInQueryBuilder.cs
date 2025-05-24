using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Utilities;

namespace TimeTracker.UnitTests.Common.Builders.Application.Auth
{
    public class SignInQueryBuilder : EntityBuilder<SignInQueryBuilder, SignIn.Query>
    {
        private string _email = RandomStringGenerator.GenerateEmail(10);
        private string _password = RandomStringGenerator.GenerateString(10, 30);

        protected override SignIn.Query Instance => new SignIn.Query(_email, _password);

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
    }
}
