using FluentAssertions;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Repositories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class CategoryRepositoryTests : IClassFixture<DataTestFixture>
{
    private readonly CategoryRepository _sut;
    private readonly DataTestFixture _fixture;

    public CategoryRepositoryTests(DataTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new CategoryRepository(_fixture.Context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories_WhenCategoriesExist()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var categories = new[]
        {
            new CategoryBuilder().WithOrganizationId(organization.Id).Build(),
            new CategoryBuilder().WithOrganizationId(organization.Id).Build(),
            new CategoryBuilder().WithOrganizationId(organization.Id).Build()
        };
        _fixture.Seed<Guid>(categories);

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().OnlyContain(c => c.OrganizationId == organization.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoCategoriesExist()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder()
            .WithOrganizationId(organization.Id)
            .WithName("Test Category")
            .Build();
        _fixture.Seed<Guid>(new[] { category });

        // Act
        var result = await _sut.GetByIdAsync(category.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.Name.Should().Be("Test Category");
        result.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenCategoryExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        _fixture.Seed<Guid>(new[] { category });

        // Act
        var result = await _sut.ExistsAsync(category.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertCategory_WhenValidCategoryProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder()
            .WithOrganizationId(organization.Id)
            .WithName("New Category")
            .Build();

        // Act
        var result = await _sut.InsertAsync(category, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedCategory = await _sut.GetByIdAsync(category.Id, CancellationToken.None);
        insertedCategory.Should().NotBeNull();
        insertedCategory!.Name.Should().Be("New Category");
    }

    [Fact]
    public async Task InsertAsync_ShouldThrowArgumentNullException_WhenCategoryIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.InsertAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenValidCategoryProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder()
            .WithOrganizationId(organization.Id)
            .WithName("Original Name")
            .Build();
        _fixture.Seed<Guid>(new[] { category });

        // Modify the category
        category.Name = "Updated Name";
        category.Description = "Updated Description";

        // Act
        var result = await _sut.UpdateAsync(category, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var updatedCategory = await _sut.GetByIdAsync(category.Id, CancellationToken.None);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("Updated Name");
        updatedCategory.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenCategoryIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.UpdateAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCategory_WhenValidCategoryProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        _fixture.Seed<Guid>(new[] { category });

        // Act
        var result = await _sut.DeleteAsync(category, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var deletedCategory = await _sut.GetByIdAsync(category.Id, CancellationToken.None);
        deletedCategory.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenCategoryIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.DeleteAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task InsertAsync_WithPersistFalse_ShouldNotPersistChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();

        // Act
        var result = await _sut.InsertAsync(category, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        var insertedCategory = await _sut.GetByIdAsync(category.Id, CancellationToken.None);
        insertedCategory.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();

        // Add entity without persisting
        await _sut.InsertAsync(category, false, CancellationToken.None);

        // Act
        var result = await _sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var savedCategory = await _sut.GetByIdAsync(category.Id, CancellationToken.None);
        savedCategory.Should().NotBeNull();
    }
}