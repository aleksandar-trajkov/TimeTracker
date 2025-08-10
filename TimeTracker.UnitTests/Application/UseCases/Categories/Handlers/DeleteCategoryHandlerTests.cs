using FluentAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Categories.Handlers;

public class DeleteCategoryHandlerTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly DeleteCategoryHandler _sut;

    public DeleteCategoryHandlerTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _sut = new DeleteCategoryHandler(_categoryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldDeleteCategoryAndReturnTrue()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenDeleteAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(categoryId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenGetByIdAsync(categoryId, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        
        // Verify DeleteAsync was never called
        await _categoryRepository.VerifyDeleteAsyncWasNotCalled();
    }

    [Fact]
    public async Task Handle_WhenRepositoryDeleteFails_ShouldReturnFalse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Category to Delete")
            .Build();

        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenDeleteAsyncFails();

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

        var command = new DeleteCategoryHandler.Command(categoryId);
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenDeleteAsync(cancellationToken);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeTrue();
        
        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(categoryId, cancellationToken);
    }
}