using FluentAssertions;
using TimeTracker.Application.Helpers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Base;
using TimeTracker.Infrastructure.Data.SqlServer;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Extensions;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class UserRepositoryTests : DataTestFixture
{
    UserRepository _sut;
    private Guid _userId;
    private User _user;

    public UserRepositoryTests()
    {
        _sut = new UserRepository(Context);

        _userId = Guid.NewGuid();
        _user = UserBuilder.Build(x => x
            .WithId(_userId)
            .WithPermissions(ListHelper.CreateList<Domain.Auth.Permission>(
                PermissionBuilder.Build(x => x.WithKey(PermissionEnum.CanEditOwnRecord).WithUserId(_userId)),
                PermissionBuilder.Build(x => x.WithKey(PermissionEnum.CanEditAnyRecord).WithUserId(_userId))
        ).AsEnumerable()));
        this.Seed<Guid>(ListHelper.CreateList(_user));

        var permissions = (ICollection<Domain.Auth.Permission>)_user.Permissions;
        this.Seed<Guid>(permissions);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {

        var result = await _sut.GetByEmailAsync(_user.Email, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(_user.Id);
        result.Email.Should().Be(_user.Email);
        result.Permissions.Should().NotBeNullOrEmpty();
        result.Permissions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        var result = async () => await _sut.GetByEmailAsync(Random.Shared.GenerateEmail(10), CancellationToken.None);

        var exception = await Record.ExceptionAsync(result);
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExists()
    {
        var result = await _sut.ExistsByEmailAsync(_user.Email, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        var result = await _sut.ExistsByEmailAsync(Random.Shared.GenerateEmail(10), CancellationToken.None);
        result.Should().BeFalse();
    }
}