using AwesomeAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Application.Categories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Categories;

public class UpdateCategoryHandlerTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly MediatorMockDouble _mediator;
    private readonly UpdateCategoryHandler _sut;

    public UpdateCategoryHandlerTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _mediator = new MediatorMockDouble();
        _sut = new UpdateCategoryHandler(_categoryRepository.Instance, _mediator.Instance);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldUpdateCategoryAndReturnTrue()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Original Name")
            .WithDescription("Original Description")
            .Build();

        var command = new UpdateCategoryCommandBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription("Updated Description")
            .Build();

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingCategory);

        var expectedCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription("Updated Description")
            .Build();

        await _categoryRepository.VerifyUpdateAsyncWasCalledWith(expectedCategory, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenUpdateWithNullDescription_ShouldUpdateCategoryWithNullDescription()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Original Name")
            .WithDescription("Original Description")
            .Build();

        var command = new UpdateCategoryCommandBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription(null)
            .Build();

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingCategory);

        var expectedCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription(null)
            .Build();

        await _categoryRepository.VerifyUpdateAsyncWasCalledWith(expectedCategory, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenRepositoryUpdateFails_ShouldReturnFalse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Original Name")
            .Build();

        var command = new UpdateCategoryCommandBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription("Updated Description")
            .Build();

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsyncFails();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(existingCategory);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Original Name")
            .Build();

        var command = new UpdateCategoryCommandBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription("Updated Description")
            .Build();
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsync(cancellationToken);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeSameAs(existingCategory);

        var expectedCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription("Updated Description")
            .Build();

        await _categoryRepository.VerifyUpdateAsyncWasCalledWith(expectedCategory, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequestedOnGetById_ShouldPassCancellationToken()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Original Name")
            .Build();

        var command = new UpdateCategoryCommandBuilder()
            .WithId(categoryId)
            .WithName("Updated Name")
            .WithDescription("Updated Description")
            .Build();
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeSameAs(existingCategory);

        // Verify GetByIdAsync was called with the cancellation token
        await _categoryRepository.VerifyGetByIdAsyncWasCalledWith(categoryId, cancellationToken);
    }
}
