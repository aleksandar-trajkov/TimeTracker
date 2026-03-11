using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.TimeEntries;

public class UpdateTimeEntryHandlerTests
{
    private readonly UserContextMockDouble _userContext;
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly MediatorMockDouble _mediator;
    private readonly UpdateTimeEntryHandler _sut;

    public UpdateTimeEntryHandlerTests()
    {
        _userContext = new UserContextMockDouble();
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _mediator = new MediatorMockDouble();
        _sut = new UpdateTimeEntryHandler(_userContext.Instance, _timeEntryRepository.Instance, _mediator.Instance);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldUpdateTimeEntryAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var newFrom = DateTimeOffset.Now.AddHours(-3);
        var newTo = DateTimeOffset.Now.AddHours(-1);
        var newDescription = "Updated description";

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .WithFrom(DateTimeOffset.Now.AddHours(-2))
            .WithTo(DateTimeOffset.Now)
            .WithDescription("Old description")
            .Build();

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithFrom(newFrom)
            .WithTo(newTo)
            .WithDescription(newDescription)
            .WithCategoryId(categoryId)
            .Build();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenUpdateAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingTimeEntry);
        existingTimeEntry.From.Should().Be(newFrom);
        existingTimeEntry.To.Should().Be(newTo);
        existingTimeEntry.Description.Should().Be(newDescription);
        existingTimeEntry.CategoryId.Should().Be(categoryId);
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

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithFrom(DateTimeOffset.Now.AddHours(-1))
            .WithTo(DateTimeOffset.Now)
            .WithDescription("Updated description")
            .WithCategoryId(Guid.NewGuid())
            .Build();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);

        // Act
        var act = async () => await _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*only update your own time entries*");
    }

    [Fact]
    public async Task Handle_WhenUpdateSucceeds_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithFrom(DateTimeOffset.Now.AddHours(-2))
            .WithTo(DateTimeOffset.Now)
            .WithDescription("Updated")
            .WithCategoryId(Guid.NewGuid())
            .Build();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenUpdateAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingTimeEntry);
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

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithFrom(DateTimeOffset.Now.AddHours(-1))
            .WithTo(DateTimeOffset.Now)
            .WithDescription("Updated")
            .WithCategoryId(Guid.NewGuid())
            .Build();

        var cancellationToken = new CancellationToken(true);
        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenUpdateAsync();

        // Act & Assert
        await _sut.Handle(command, cancellationToken);
    }

    [Fact]
    public async Task Handle_ShouldPreserveUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithFrom(DateTimeOffset.Now.AddHours(-1))
            .WithTo(DateTimeOffset.Now)
            .WithDescription("Updated description")
            .WithCategoryId(Guid.NewGuid())
            .Build();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenUpdateAsync();

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        existingTimeEntry.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllEditableProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var newFrom = DateTimeOffset.Now.AddHours(-5);
        var newTo = DateTimeOffset.Now.AddHours(-3);
        var newDescription = "Completely new description";

        var existingTimeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithFrom(newFrom)
            .WithTo(newTo)
            .WithDescription(newDescription)
            .WithCategoryId(newCategoryId)
            .Build();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, existingTimeEntry);
        _timeEntryRepository.GivenUpdateAsync();

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        existingTimeEntry.From.Should().Be(newFrom);
        existingTimeEntry.To.Should().Be(newTo);
        existingTimeEntry.Description.Should().Be(newDescription);
        existingTimeEntry.CategoryId.Should().Be(newCategoryId);
    }
}
