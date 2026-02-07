using FluentAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.TimeEntries;

public class GetTimeEntryByIdHandlerTests
{
    private readonly UserContextMockDouble _userContext;
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly GetTimeEntryByIdHandler _sut;

    public GetTimeEntryByIdHandlerTests()
    {
        _userContext = new UserContextMockDouble();
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _sut = new GetTimeEntryByIdHandler(_userContext.Instance, _timeEntryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenValidQueryForOwnEntry_ShouldReturnTimeEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddHours(-2);
        var to = DateTimeOffset.Now;
        var description = "Test entry";

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .WithFrom(from)
            .WithTo(to)
            .WithDescription(description)
            .WithCategoryId(categoryId)
            .Build();

        var query = new GetTimeEntryByIdHandler.Query(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(timeEntryId);
        result.UserId.Should().Be(userId);
        result.From.Should().Be(from);
        result.To.Should().Be(to);
        result.Description.Should().Be(description);
        result.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnTimeEntry_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(otherUserId)
            .Build();

        var query = new GetTimeEntryByIdHandler.Query(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*only access your own time entries*");
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var query = new GetTimeEntryByIdHandler.Query(timeEntryId);
        var cancellationToken = new CancellationToken(true);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act & Assert
        await _sut.Handle(query, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenUserOwnsEntry_ShouldReturnCompleteTimeEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var query = new GetTimeEntryByIdHandler.Query(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(timeEntry);
    }

    [Fact]
    public async Task Handle_WhenUserOwnsEntry_ShouldNotThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var timeEntry = new TimeEntryBuilder()
            .WithId(timeEntryId)
            .WithUserId(userId)
            .Build();

        var query = new GetTimeEntryByIdHandler.Query(timeEntryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetByIdAsync(timeEntryId, timeEntry);

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
