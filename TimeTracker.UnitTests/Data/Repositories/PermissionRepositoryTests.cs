using FluentAssertions;
using TimeTracker.Domain.Auth;
using TimeTracker.Infrastructure.Data.SqlServer.Repositories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class PermissionRepositoryTests : IClassFixture<DataTestFixture>
{
    private readonly PermissionRepository _sut;
    private readonly DataTestFixture _fixture;

    public PermissionRepositoryTests(DataTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new PermissionRepository(_fixture.Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPermission_WhenPermissionExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();

        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var permission = new PermissionBuilder()
            .WithUserId(user.Id)
            .WithKey(PermissionEnum.CanEditOwnRecord)
            .Build();
        _fixture.Seed<Guid>(new[] { permission });

        // Act
        var result = await _sut.GetByIdAsync(permission.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(permission.Id);
        result.Key.Should().Be(PermissionEnum.CanEditOwnRecord);
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPermissionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenPermissionExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();

        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var permission = new PermissionBuilder().WithUserId(user.Id).Build();
        _fixture.Seed<Guid>(new[] { permission });

        // Act
        var result = await _sut.ExistsAsync(permission.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenPermissionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertPermission_WhenValidPermissionProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();

        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var permission = new PermissionBuilder()
            .WithUserId(user.Id)
            .WithKey(PermissionEnum.CanEditOwnRecord)
            .Build();

        // Act
        var result = await _sut.InsertAsync(permission, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedPermission = await _sut.GetByIdAsync(permission.Id, CancellationToken.None);
        insertedPermission.Should().NotBeNull();
        insertedPermission!.Key.Should().Be(PermissionEnum.CanEditOwnRecord);
        insertedPermission.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task InsertAsync_ShouldThrowArgumentNullException_WhenPermissionIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.InsertAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenPermissionIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.UpdateAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePermission_WhenValidPermissionProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();

        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var permission = new PermissionBuilder().WithUserId(user.Id).Build();
        _fixture.Seed<Guid>(new[] { permission });

        // Act
        var result = await _sut.DeleteAsync(permission, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var deletedPermission = await _sut.GetByIdAsync(permission.Id, CancellationToken.None);
        deletedPermission.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenPermissionIsNull()
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

        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var permission = new PermissionBuilder().WithUserId(user.Id).Build();

        // Act
        var result = await _sut.InsertAsync(permission, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        var insertedPermission = await _sut.GetByIdAsync(permission.Id, CancellationToken.None);
        insertedPermission.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();

        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var permission = new PermissionBuilder().WithUserId(user.Id).Build();

        // Add entity without persisting
        await _sut.InsertAsync(permission, false, CancellationToken.None);

        // Act
        var result = await _sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var savedPermission = await _sut.GetByIdAsync(permission.Id, CancellationToken.None);
        savedPermission.Should().NotBeNull();
    }
}