using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Auth;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Auth.TheoryData
{
    public class IsSignInRequestValidTheoryData : TheoryData<SignIn.Query, string>
    {
        public IsSignInRequestValidTheoryData()
        {
            Add(SignInQueryBuilder.Build(x => x.WithEmail(string.Empty)), "Email is required.");
            Add(SignInQueryBuilder.Build(x => x.WithEmail("invalid-email-format")), "Invalid email format.");
            Add(SignInQueryBuilder.Build(x => x.WithPassword(string.Empty)), "Password is required");
            Add(SignInQueryBuilder.Build(x => x.WithPassword("short")), "Password must be at least 8 characters long.");
            Add(SignInQueryBuilder.Build(), "User with email does not exist in system");
        }
    }
}
