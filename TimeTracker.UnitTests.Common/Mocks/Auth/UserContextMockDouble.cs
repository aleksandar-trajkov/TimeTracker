using NSubstitute;
using TimeTracker.Application.Interfaces.Auth;

namespace TimeTracker.UnitTests.Common.Mocks.Auth;

public class UserContextMockDouble : MockDouble<IUserContext>
{
    public void GivenUserId(Guid userId)
    {
        Instance.UserId.Returns(userId);
    }
}
