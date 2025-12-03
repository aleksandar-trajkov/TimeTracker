using FluentAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.TimeEntries;

public class CreateTimeEntryHandlerTests
{
    private readonly UserContextMockDouble _userContext;
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly CreateTimeEntryHandler _sut;

    public CreateTimeEntryHandlerTests()
    {
        _userContext = new UserContextMockDouble();
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _sut = new CreateTimeEntryHandler(_userContext.Instance, _timeEntryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCreateTimeEntryAndReturnIt()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddHours(-2);
        var to = DateTimeOffset.Now;
        var description = "Working on feature X";

        var command = new CreateTimeEntryHandler.Command(from, to, description, categoryId);

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.From.Should().Be(from);
        result.To.Should().Be(to);
        result.Description.Should().Be(description);
        result.CategoryId.Should().Be(categoryId);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_ShouldAssignCurrentUserIdToTimeEntry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTimeEntryHandler.Command(
            DateTimeOffset.Now.AddHours(-1),
            DateTimeOffset.Now,
            "Test entry",
            Guid.NewGuid());

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_ShouldGenerateNewGuidForTimeEntry()
    {
        // Arrange
        var command = new CreateTimeEntryHandler.Command(
            DateTimeOffset.Now.AddHours(-1),
            DateTimeOffset.Now,
            "Test entry",
            Guid.NewGuid());

        _userContext.GivenUserId(Guid.NewGuid());
        _timeEntryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var command = new CreateTimeEntryHandler.Command(
            DateTimeOffset.Now.AddHours(-1),
            DateTimeOffset.Now,
            "Test entry",
            Guid.NewGuid());

        var cancellationToken = new CancellationToken(true);
        _userContext.GivenUserId(Guid.NewGuid());
        _timeEntryRepository.GivenInsertAsync();

        // Act & Assert
        await _sut.Handle(command, cancellationToken);
    }

    [Fact]
    public async Task Handle_WithShortDescription_ShouldCreateTimeEntry()
    {
        // Arrange
        var command = new CreateTimeEntryHandler.Command(
            DateTimeOffset.Now.AddHours(-1),
            DateTimeOffset.Now,
            "A",
            Guid.NewGuid());

        _userContext.GivenUserId(Guid.NewGuid());
        _timeEntryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Description.Should().Be("A");
    }

    [Fact]
    public async Task Handle_WithLongDescription_ShouldCreateTimeEntry()
    {
        // Arrange
        var longDescription = new string('x', 500);
        var command = new CreateTimeEntryHandler.Command(
            DateTimeOffset.Now.AddHours(-1),
            DateTimeOffset.Now,
            longDescription,
            Guid.NewGuid());

        _userContext.GivenUserId(Guid.NewGuid());
        _timeEntryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Description.Should().Be(longDescription);
    }
}
