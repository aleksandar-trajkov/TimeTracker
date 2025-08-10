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
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Category to Delete")
            .WithDescription("Category Description")
            .Build();

        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenDeleteAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        var expectedCategory = new CategoryBuilder()
            .WithId(categoryId)
            .Build();
        
        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(expectedCategory, CancellationToken.None);
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
        await _categoryRepository.Instance.DidNotReceive().DeleteAsync(
            Arg.Any<Category>(), 
            Arg.Any<bool>(), 
            Arg.Any<CancellationToken>());
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
        _categoryRepository.Instance.DeleteAsync(Arg.Any<Category>(), true, Arg.Any<CancellationToken>())
            .Returns(0); // Simulate delete failure

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
            .WithName("Category to Delete")
            .Build();

        var command = new DeleteCategoryHandler.Command(categoryId);
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenDeleteAsync(cancellationToken);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeTrue();
        
        var expectedCategory = new CategoryBuilder()
            .WithId(categoryId)
            .Build();
        
        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(expectedCategory, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequestedOnGetById_ShouldPassCancellationToken()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryHandler.Command(categoryId);
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.Instance.GetByIdAsync(categoryId, cancellationToken)
            .Returns((Category?)null);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeFalse();
        
        // Verify GetByIdAsync was called with the cancellation token
        await _categoryRepository.Instance.Received(1).GetByIdAsync(categoryId, cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenValidCategoryWithoutDescription_ShouldDeleteSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Category without Description")
            .WithoutDescription()
            .Build();

        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenDeleteAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        var expectedCategory = new CategoryBuilder()
            .WithId(categoryId)
            .Build();
        
        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(expectedCategory, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenMultipleCallsWithSameId_ShouldHandleConsistently()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new CategoryBuilder()
            .WithId(categoryId)
            .WithName("Consistent Category")
            .Build();

        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenGetByIdAsync(categoryId, existingCategory);
        _categoryRepository.GivenDeleteAsync();

        // Act
        var result1 = await _sut.Handle(command, CancellationToken.None);
        var result2 = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        
        // Verify both calls were made
        await _categoryRepository.Instance.Received(2).GetByIdAsync(categoryId, Arg.Any<CancellationToken>());
        await _categoryRepository.Instance.Received(2).DeleteAsync(Arg.Any<Category>(), true, Arg.Any<CancellationToken>());
    }
}