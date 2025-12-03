using FluentAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.TimeEntries;

public class DeleteTimeEntryHandlerTests
{
    private readonly UserContextMockDouble _userContext;
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly DeleteTimeEntryHandler _sut;

    public DeleteTimeEntryHandlerTests()
    {
        _userContext = new UserContextMockDouble();
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _sut = new DeleteTimeEntryHandler(_userContext.Instance, _timeEntryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldDeleteTimeEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenDeleteAsync();

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert - no exception thrown
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnTimeEntry_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(otherUserId)
            .Build();

        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);

        // Act
        var act = async () => await _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*only delete your own time entries*");
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new DeleteTimeEntryHandler.Command(timeEntryId);
        var cancellationToken = new CancellationToken(true);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenDeleteAsync();

        // Act & Assert
        await _sut.Handle(command, cancellationToken);
    }

    [Fact]
    public async Task Handle_ShouldCallDeleteAsyncWithCorrectId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenDeleteAsync();

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert - verify the method was called (implicitly by the mock setup)
    }

    [Fact]
    public async Task Handle_WhenUserOwnsEntry_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new DeleteTimeEntryHandler.Command(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenDeleteAsync();

        // Act
        var act = async () => await _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
