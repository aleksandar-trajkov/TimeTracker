using AwesomeAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Categories;

public class DeleteCategoryHandlerTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly MediatorMockDouble _mediator;
    private readonly DeleteCategoryHandler _sut;

    public DeleteCategoryHandlerTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _mediator = new MediatorMockDouble();
        _sut = new DeleteCategoryHandler(_categoryRepository.Instance, _mediator.Instance);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldCallDeleteCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var command = new DeleteCategoryHandler.Command(categoryId);

        _categoryRepository.GivenDeleteAsync();

        // Act
        await _sut.Handle(command, CancellationToken.None);

        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(categoryId, CancellationToken.None);
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
        await _sut.Handle(command, cancellationToken);

        await _categoryRepository.VerifyDeleteAsyncWasCalledWith(categoryId, cancellationToken);
    }
}
