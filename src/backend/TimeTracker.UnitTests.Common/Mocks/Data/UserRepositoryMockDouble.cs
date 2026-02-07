using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Mocks.Data;

public class UserRepositoryMockDouble : MockDouble<IUserRepository>
{
    public void GivenGetAll(Guid organizationId, IEnumerable<User> users)
    {
        Instance.GetAllAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(users);
    }

    public void GivenGetByIdAsync(Guid userId, User user)
    {
        Instance.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
    }

    public void GivenGetByEmail(string email, User user)
    {
        Instance.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .ReturnsAsync(user);
    }

    public void GivenGetByEmailThrows(string email, Exception exception)
    {
        Instance.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);
    }

    public void GivenExistsByEmail(string email, bool exists)
    {
        Instance.ExistsByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(exists);
    }
}
