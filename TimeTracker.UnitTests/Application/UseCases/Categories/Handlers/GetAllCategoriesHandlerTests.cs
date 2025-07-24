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
        var organizationId = Guid.NewGuid();
        // Arrange
        var categories = new List<Category>
        {
            new CategoryBuilder().WithName("Work").WithDescription("Work related tasks").WithOrganizationId(organizationId).Build(),
            new CategoryBuilder().WithName("Personal").WithDescription("Personal tasks").WithOrganizationId(organizationId).Build(),
            new CategoryBuilder().WithName("Education").WithoutDescription().WithOrganizationId(organizationId).Build()
        };

        _categoryRepository.GivenGetAllAsync(organizationId, categories);

        var query = new GetAllCategoriesHandler.Query(organizationId);

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
        var organizationId = Guid.NewGuid();
        // Arrange
        var emptyCategories = new List<Category>();
        _categoryRepository.GivenGetAllAsync(organizationId, emptyCategories);

        var query = new GetAllCategoriesHandler.Query(organizationId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}