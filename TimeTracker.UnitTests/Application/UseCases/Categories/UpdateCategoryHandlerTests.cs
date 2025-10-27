using FluentAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Categories;

public class UpdateCategoryHandlerTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly UpdateCategoryHandler _sut;

    public UpdateCategoryHandlerTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _sut = new UpdateCategoryHandler(_categoryRepository.Instance);
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

        var command = new UpdateCategoryHandler.Command(categoryId, "Updated Name", "Updated Description");

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

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

        var command = new UpdateCategoryHandler.Command(categoryId, "Updated Name", null);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

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

        var command = new UpdateCategoryHandler.Command(categoryId, "Updated Name", "Updated Description");

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsyncFails();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
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

        var command = new UpdateCategoryHandler.Command(categoryId, "Updated Name", "Updated Description");
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenUpdateAsync(cancellationToken);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeTrue();

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

        var command = new UpdateCategoryHandler.Command(categoryId, "Updated Name", "Updated Description");
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeFalse();

        // Verify GetByIdAsync was called with the cancellation token
        await _categoryRepository.VerifyGetByIdAsyncWasCalledWith(categoryId, cancellationToken);
    }
}