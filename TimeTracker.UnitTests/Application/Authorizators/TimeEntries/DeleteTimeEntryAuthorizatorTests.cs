using FluentAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Authorizators;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Authorizators.TimeEntries;

public class DeleteTimeEntryAuthorizatorTests
{
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly UserRepositoryMockDouble _userRepository;
    private readonly UserContextMockDouble _userContext;
    private readonly DeleteTimeEntryAuthorizator _sut;

    public DeleteTimeEntryAuthorizatorTests()
    {
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _userRepository = new UserRepositoryMockDouble();
        _userContext = new UserContextMockDouble();
        _sut = new DeleteTimeEntryAuthorizator(
            _timeEntryRepository.Instance,
            _userRepository.Instance,
            _userContext.Instance);
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserDeletesOwnTimeEntryWithCanEditOwnPermission_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithPermissions(new[]
            {
                new PermissionBuilder()
                    .WithKey(PermissionEnum.CanEditOwnTimeEntry)
                    .WithUserId(userId)
                    .Build()
            })
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserDeletesOwnTimeEntryWithCanEditAnyPermission_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithPermissions(new[]
            {
                new PermissionBuilder()
                    .WithKey(PermissionEnum.CanEditAnyTimeEntry)
                    .WithUserId(userId)
                    .Build()
            })
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserDeletesOthersTimeEntryWithCanEditAnyPermission_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(otherUserId)
            .Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithPermissions(new[]
            {
                new PermissionBuilder()
                    .WithKey(PermissionEnum.CanEditAnyTimeEntry)
                    .WithUserId(userId)
                    .Build()
            })
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserDeletesOwnTimeEntryWithoutPermission_ShouldThrowAuthorizationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithPermissions(Array.Empty<Permission>())
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage("User does not have permission to delete own time entries.");
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserDeletesOthersTimeEntryWithOnlyCanEditOwnPermission_ShouldThrowAuthorizationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(otherUserId)
            .Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithPermissions(new[]
            {
                new PermissionBuilder()
                    .WithKey(PermissionEnum.CanEditOwnTimeEntry)
                    .WithUserId(userId)
                    .Build()
            })
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage("User does not have permission to delete other users' time entries.");
    }

    [Fact]
    public async Task AuthorizeAsync_WhenUserDeletesOthersTimeEntryWithoutPermission_ShouldThrowAuthorizationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(otherUserId)
            .Build();

        var user = new UserBuilder()
            .WithId(userId)
            .WithEmail("test@example.com")
            .WithPermissions(Array.Empty<Permission>())
            .Build();

        _userContext.GivenUserId(userId);
        _userRepository.GivenGetByIdAsync(userId, user);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.AuthorizeAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthorizationException>()
            .WithMessage("User does not have permission to delete other users' time entries.");
    }
}
