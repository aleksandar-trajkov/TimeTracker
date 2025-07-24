using FluentAssertions;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Repositories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class OrganizationRepositoryTests : IClassFixture<DataTestFixture>
{
    private readonly OrganizationRepository _sut;
    private readonly DataTestFixture _fixture;

    public OrganizationRepositoryTests(DataTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new OrganizationRepository(_fixture.Context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrganizations_WhenOrganizationsExist()
    {
        // Arrange
        var organizations = new[]
        {
            new OrganizationBuilder().WithName("Organization 1").Build(),
            new OrganizationBuilder().WithName("Organization 2").Build(),
            new OrganizationBuilder().WithName("Organization 3").Build()
        };
        _fixture.Seed<Guid>(organizations);

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(o => o.Name == "Organization 1");
        result.Should().Contain(o => o.Name == "Organization 2");
        result.Should().Contain(o => o.Name == "Organization 3");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoOrganizationsExist()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrganization_WhenOrganizationExists()
    {
        // Arrange
        var organization = new OrganizationBuilder()
            .WithName("Test Organization")
            .WithDescription("Test Description")
            .Build();
        _fixture.Seed<Guid>(new[] { organization });

        // Act
        var result = await _sut.GetByIdAsync(organization.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(organization.Id);
        result.Name.Should().Be("Test Organization");
        result.Description.Should().Be("Test Description");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrganizationDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenOrganizationExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        // Act
        var result = await _sut.ExistsAsync(organization.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenOrganizationDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertOrganization_WhenValidOrganizationProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder()
            .WithName("New Organization")
            .WithDescription("New Description")
            .Build();

        // Act
        var result = await _sut.InsertAsync(organization, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedOrganization = await _sut.GetByIdAsync(organization.Id, CancellationToken.None);
        insertedOrganization.Should().NotBeNull();
        insertedOrganization!.Name.Should().Be("New Organization");
        insertedOrganization.Description.Should().Be("New Description");
    }

    [Fact]
    public async Task InsertAsync_ShouldThrowArgumentNullException_WhenOrganizationIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.InsertAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrganization_WhenValidOrganizationProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder()
            .WithName("Original Name")
            .WithDescription("Original Description")
            .Build();
        _fixture.Seed<Guid>(new[] { organization });

        // Modify the organization
        organization.Name = "Updated Name";
        organization.Description = "Updated Description";

        // Act
        var result = await _sut.UpdateAsync(organization, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var updatedOrganization = await _sut.GetByIdAsync(organization.Id, CancellationToken.None);
        updatedOrganization.Should().NotBeNull();
        updatedOrganization!.Name.Should().Be("Updated Name");
        updatedOrganization.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenOrganizationIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.UpdateAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteOrganization_WhenValidOrganizationProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(new[] { organization });

        // Act
        var result = await _sut.DeleteAsync(organization, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var deletedOrganization = await _sut.GetByIdAsync(organization.Id, CancellationToken.None);
        deletedOrganization.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenOrganizationIsNull()
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

        // Act
        var result = await _sut.InsertAsync(organization, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        var insertedOrganization = await _sut.GetByIdAsync(organization.Id, CancellationToken.None);
        insertedOrganization.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();

        // Add entity without persisting
        await _sut.InsertAsync(organization, false, CancellationToken.None);

        // Act
        var result = await _sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var savedOrganization = await _sut.GetByIdAsync(organization.Id, CancellationToken.None);
        savedOrganization.Should().NotBeNull();
    }
}