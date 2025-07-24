using FluentAssertions;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Repositories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class TimeEntryRepositoryTests : IClassFixture<DataTestFixture>
{
    private readonly TimeEntryRepository _sut;
    private readonly DataTestFixture _fixture;

    public TimeEntryRepositoryTests(DataTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new TimeEntryRepository(_fixture.Context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTimeEntries_WhenTimeEntriesExist()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntries = new[]
        {
            new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build(),
            new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build(),
            new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build()
        };
        _fixture.Seed<Guid>(timeEntries);

        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().OnlyContain(te => te.UserId == user.Id && te.CategoryId == category.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoTimeEntriesExist()
    {
        // Act
        var result = await _sut.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTimeEntry_WhenTimeEntryExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder()
            .WithUserId(user.Id)
            .WithCategoryId(category.Id)
            .WithDescription("Test Time Entry")
            .Build();
        _fixture.Seed<Guid>(new[] { timeEntry });

        // Act
        var result = await _sut.GetByIdAsync(timeEntry.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(timeEntry.Id);
        result.Description.Should().Be("Test Time Entry");
        result.UserId.Should().Be(user.Id);
        result.CategoryId.Should().Be(category.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTimeEntryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenTimeEntryExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build();
        _fixture.Seed<Guid>(new[] { timeEntry });

        // Act
        var result = await _sut.ExistsAsync(timeEntry.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenTimeEntryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertTimeEntry_WhenValidTimeEntryProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder()
            .WithUserId(user.Id)
            .WithCategoryId(category.Id)
            .WithDescription("New Time Entry")
            .Build();

        // Act
        var result = await _sut.InsertAsync(timeEntry, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedTimeEntry = await _sut.GetByIdAsync(timeEntry.Id, CancellationToken.None);
        insertedTimeEntry.Should().NotBeNull();
        insertedTimeEntry!.Description.Should().Be("New Time Entry");
    }

    [Fact]
    public async Task InsertAsync_ShouldThrowArgumentNullException_WhenTimeEntryIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.InsertAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTimeEntry_WhenValidTimeEntryProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder()
            .WithUserId(user.Id)
            .WithCategoryId(category.Id)
            .WithDescription("Original Description")
            .Build();
        _fixture.Seed<Guid>(new[] { timeEntry });

        // Modify the time entry
        timeEntry.Description = "Updated Description";
        var newEndTime = DateTimeOffset.Now;
        timeEntry.To = newEndTime;

        // Act
        var result = await _sut.UpdateAsync(timeEntry, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var updatedTimeEntry = await _sut.GetByIdAsync(timeEntry.Id, CancellationToken.None);
        updatedTimeEntry.Should().NotBeNull();
        updatedTimeEntry!.Description.Should().Be("Updated Description");
        updatedTimeEntry.To.Should().BeCloseTo(newEndTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenTimeEntryIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.UpdateAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTimeEntry_WhenValidTimeEntryProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build();
        _fixture.Seed<Guid>(new[] { timeEntry });

        // Act
        var result = await _sut.DeleteAsync(timeEntry, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var deletedTimeEntry = await _sut.GetByIdAsync(timeEntry.Id, CancellationToken.None);
        deletedTimeEntry.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenTimeEntryIsNull()
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
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build();

        // Act
        var result = await _sut.InsertAsync(timeEntry, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        var insertedTimeEntry = await _sut.GetByIdAsync(timeEntry.Id, CancellationToken.None);
        insertedTimeEntry.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        var category = new CategoryBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });
        _fixture.Seed<Guid>(new[] { category });

        var timeEntry = new TimeEntryBuilder().WithUserId(user.Id).WithCategoryId(category.Id).Build();

        // Add entity without persisting
        await _sut.InsertAsync(timeEntry, false, CancellationToken.None);

        // Act
        var result = await _sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var savedTimeEntry = await _sut.GetByIdAsync(timeEntry.Id, CancellationToken.None);
        savedTimeEntry.Should().NotBeNull();
    }
}