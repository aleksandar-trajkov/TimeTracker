using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Authorizators;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Authorizators.TimeEntries;

public class CreateTimeEntryAuthorizatorTests
{
    private readonly UserRepositoryMockDouble _userRepository;
    private readonly UserContextMockDouble _userContext;
    private readonly CreateTimeEntryAuthorizator _sut;

    public CreateTimeEntryAuthorizatorTests()
    {
        _userRepository = new UserRepositoryMockDouble();
        _userContext = new UserContextMockDouble();
        _sut = new CreateTimeEntryAuthorizator(_userRepository.Instance, _userContext.Instance);
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserHasCanEditOwnTimeEntryPermission_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new CreateTimeEntryCommandBuilder().Build();

        var user = new UserBuilder()
            .WithPermissions(new PermissionBuilder()
                .WithKey(PermissionEnum.CanEditOwnTimeEntry)
                .Build())
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserHasCanEditAnyTimeEntryPermission_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new CreateTimeEntryCommandBuilder().Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithPermissions(new PermissionBuilder()
                .WithKey(PermissionEnum.CanEditAnyTimeEntry)
                .Build())
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserHasNoPermissions_ShouldThrowAuthorizationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new CreateTimeEntryCommandBuilder().Build();

        var user = new UserBuilder().Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage("*does not have permission to create time entries*");
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserHasWrongPermission_ShouldThrowAuthorizationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var command = new CreateTimeEntryCommandBuilder().Build();

        var user = new UserBuilder()
            .WithPermissions(new PermissionBuilder()
                .WithKey(PermissionEnum.CanEditOrganizationTimeEntries)
                .Build())
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage("*does not have permission to create time entries*");
    }
}
