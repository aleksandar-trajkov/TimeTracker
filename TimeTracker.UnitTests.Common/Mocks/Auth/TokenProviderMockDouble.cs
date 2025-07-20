using NSubstitute;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Domain.Auth;

namespace TimeTracker.UnitTests.Common.Mocks.Auth;

public class TokenProviderMockDouble : MockDouble<ITokenProvider>
{
    public void GivenGenerateToken(User user, string token)
    {
        Instance.CreateAuthToken(user).Returns(token);
    }
    public void GivenGenerateRefreshToken(string email, string refreshToken)
    {
        Instance.CreateRememberMeToken(email).Returns(refreshToken);
    }

    public void GivenTryGetLoginEmailFromRememberMeToken(string? rememberMeToken, string? email)
    {
        Instance.TryGetLoginEmailFromRememberMeToken(rememberMeToken).Returns(email);
    }
}