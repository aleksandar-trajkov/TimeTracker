using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Auth;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Auth.TheoryData
{
    public class IsSignInRequestValidTheoryData : TheoryData<SignInRequestHandler.Query, string>
    {
        public IsSignInRequestValidTheoryData()
        {
            Add(new SignInQueryBuilder().WithEmail(string.Empty).Build(), "Email is required.");
            Add(new SignInQueryBuilder().WithEmail("invalid-email-format").Build(), "Invalid email format.");
            Add(new SignInQueryBuilder().WithPassword(string.Empty).Build(), "Password is required");
            Add(new SignInQueryBuilder().WithPassword("short").Build(), "Password must be at least 8 characters long.");
            Add(new SignInQueryBuilder().Build(), "User with email does not exist in system");
        }
    }
}
