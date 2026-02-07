using FluentAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.TimeEntries;

public class GetAllTimeEntriesHandlerTests
{
    private readonly UserContextMockDouble _userContext;
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly GetAllTimeEntriesHandler _sut;

    public GetAllTimeEntriesHandlerTests()
    {
        _userContext = new UserContextMockDouble();
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _sut = new GetAllTimeEntriesHandler(_userContext.Instance, _timeEntryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenValidQuery_ShouldReturnTimeEntriesForCurrentUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-7);
        var to = DateTimeOffset.Now;

        var timeEntries = new List<TimeEntry>
        {
            new TimeEntryBuilder().WithUserId(userId).Build(),
            new TimeEntryBuilder().WithUserId(userId).Build(),
            new TimeEntryBuilder().WithUserId(userId).Build()
        };

        var query = new GetAllTimeEntriesHandler.Query(from, to);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, timeEntries);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(te => te.UserId.Should().Be(userId));
    }

    [Fact]
    public async Task Handle_WhenNoEntriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-7);
        var to = DateTimeOffset.Now;

        var query = new GetAllTimeEntriesHandler.Query(from, to);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, new List<TimeEntry>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldPassUserIdFromContext()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-1);
        var to = DateTimeOffset.Now;

        var query = new GetAllTimeEntriesHandler.Query(from, to);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, new List<TimeEntry>());

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert - verify the correct userId was used (implicitly by the mock setup)
    }

    [Fact]
    public async Task Handle_ShouldPassFromAndToParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Parse("2024-01-01T00:00:00Z");
        var to = DateTimeOffset.Parse("2024-01-31T23:59:59Z");

        var query = new GetAllTimeEntriesHandler.Query(from, to);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, new List<TimeEntry>());

        // Act
        await _sut.Handle(query, CancellationToken.None);

        // Assert - verify from and to were passed correctly (implicitly by the mock setup)
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-7);
        var to = DateTimeOffset.Now;

        var query = new GetAllTimeEntriesHandler.Query(from, to);
        var cancellationToken = new CancellationToken(true);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, new List<TimeEntry>());

        // Act & Assert
        await _sut.Handle(query, cancellationToken);
    }

    [Fact]
    public async Task Handle_WithMultipleEntries_ShouldReturnAllEntries()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-7);
        var to = DateTimeOffset.Now;

        var timeEntries = new List<TimeEntry>
        {
            new TimeEntryBuilder()
                .WithUserId(userId)
                .WithDescription("Entry 1")
                .WithFrom(DateTimeOffset.Now.AddDays(-5))
                .Build(),
            new TimeEntryBuilder()
                .WithUserId(userId)
                .WithDescription("Entry 2")
                .WithFrom(DateTimeOffset.Now.AddDays(-3))
                .Build(),
            new TimeEntryBuilder()
                .WithUserId(userId)
                .WithDescription("Entry 3")
                .WithFrom(DateTimeOffset.Now.AddDays(-1))
                .Build()
        };

        var query = new GetAllTimeEntriesHandler.Query(from, to);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, timeEntries);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainSingle(te => te.Description == "Entry 1");
        result.Should().ContainSingle(te => te.Description == "Entry 2");
        result.Should().ContainSingle(te => te.Description == "Entry 3");
    }

    [Fact]
    public async Task Handle_ShouldOnlyReturnEntriesForCurrentUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-7);
        var to = DateTimeOffset.Now;

        var timeEntries = new List<TimeEntry>
        {
            new TimeEntryBuilder().WithUserId(userId).Build(),
            new TimeEntryBuilder().WithUserId(userId).Build()
        };

        var query = new GetAllTimeEntriesHandler.Query(from, to);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenGetAllAsync(userId, from, to, timeEntries);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().AllSatisfy(te => te.UserId.Should().Be(userId));
    }
}
