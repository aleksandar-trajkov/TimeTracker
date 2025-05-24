using NSubstitute;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Mocks.Data;

public class UserRepositoryMockDouble : MockDouble<IUserRepository>
{
    public void GivenGetByEmail(string email, User user)
    {
        Instance.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .ReturnsAsync(user);
    }

    public void GivenExistsByEmail(string email, bool exists)
    {
        Instance.ExistsByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(exists);
    }
}
