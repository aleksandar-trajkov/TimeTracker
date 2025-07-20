using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Auth;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Auth.TheoryData
{
    public class IsRememberMeSignInRequestInvalidTheoryData : TheoryData<RememberMeSignInRequestHandler.Query, string>
    {
        public IsRememberMeSignInRequestInvalidTheoryData()
        {
            Add(new RememberMeSignInQueryBuilder().WithRememberMeToken(string.Empty).Build(), "Remember me token is required.");
        }
    }
}