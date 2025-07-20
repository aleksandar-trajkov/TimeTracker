using FluentAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Categories.Handlers;

public class GetAllCategoriesHandlerTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly GetAllCategoriesHandler _sut;

    public GetAllCategoriesHandlerTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _sut = new GetAllCategoriesHandler(_categoryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new CategoryBuilder().WithName("Work").WithDescription("Work related tasks").Build(),
            new CategoryBuilder().WithName("Personal").WithDescription("Personal tasks").Build(),
            new CategoryBuilder().WithName("Education").WithoutDescription().Build()
        };

        _categoryRepository.GivenGetAllAsync(categories);

        var query = new GetAllCategoriesHandler.Query();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var emptyCategories = new List<Category>();
        _categoryRepository.GivenGetAllAsync(emptyCategories);

        var query = new GetAllCategoriesHandler.Query();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}