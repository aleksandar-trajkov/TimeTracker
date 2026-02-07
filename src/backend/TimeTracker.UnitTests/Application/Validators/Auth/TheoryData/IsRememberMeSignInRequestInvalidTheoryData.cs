using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Auth;

namespace TimeTracker.UnitTests.Application.Validators.Auth.TheoryData
{
    public class IsRememberMeSignInRequestInvalidTheoryData : TheoryData<RememberMeSignInHandler.Query, string>
    {
        public IsRememberMeSignInRequestInvalidTheoryData()
        {
            Add(new RememberMeSignInQueryBuilder().WithRememberMeToken(string.Empty).Build(), "Remember me token is required.");
        }
    }
}