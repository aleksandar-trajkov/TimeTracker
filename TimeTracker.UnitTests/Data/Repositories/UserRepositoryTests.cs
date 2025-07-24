using FluentAssertions;
using TimeTracker.Application.Helpers;
using TimeTracker.Domain;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Repositories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Extensions;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class UserRepositoryTests : IClassFixture<DataTestFixture>
{
    private readonly UserRepository _sut;
    private readonly DataTestFixture _fixture;

    public UserRepositoryTests(DataTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new UserRepository(fixture.Context);
    }

    #region UserRepository-specific methods

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var userId = Guid.NewGuid();
        var user = new UserBuilder()
            .WithId(userId)
            .WithOrganizationId(organization.Id)
            .WithEmail("test@example.com")
            .WithPermissions(ListHelper.CreateList<Domain.Auth.Permission>(
                new PermissionBuilder().WithKey(PermissionEnum.CanEditOwnRecord).WithUserId(userId).Build(),
                new PermissionBuilder().WithKey(PermissionEnum.CanEditAnyRecord).WithUserId(userId).Build())
            .AsEnumerable())
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        var permissions = (ICollection<Domain.Auth.Permission>)user.Permissions;
        _fixture.Seed<Guid>(permissions);

        // Act
        var result = await _sut.GetByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Permissions.Should().NotBeNullOrEmpty();
        result.Permissions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldThrowInvalidOperationException_WhenEmailDoesNotExist()
    {
        // Arrange
        var nonExistentEmail = Random.Shared.GenerateEmail(10);

        // Act & Assert
        var act = async () => await _sut.GetByEmailAsync(nonExistentEmail, CancellationToken.None);
        var exception = await Record.ExceptionAsync(act);
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("exists@example.com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        // Act
        var result = await _sut.ExistsByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Arrange
        var nonExistentEmail = Random.Shared.GenerateEmail(10);

        // Act
        var result = await _sut.ExistsByEmailAsync(nonExistentEmail, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region BaseRepository inherited methods

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("getbyid@example.com")
            .WithFirstName("John")
            .WithLastName("Doe")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        // Act
        var result = await _sut.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("exists2@example.com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        // Act
        var result = await _sut.ExistsAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertUser_WhenValidUserProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var newUser = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("newuser@test.com")
            .WithFirstName("New")
            .WithLastName("User")
            .Build();

        // Act
        var result = await _sut.InsertAsync(newUser, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedUser = await _sut.GetByIdAsync(newUser.Id, CancellationToken.None);
        insertedUser.Should().NotBeNull();
        insertedUser!.Email.Should().Be("newuser@test.com");
        insertedUser.FirstName.Should().Be("New");
        insertedUser.LastName.Should().Be("User");
        insertedUser.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task InsertAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.InsertAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenValidUserProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var userToUpdate = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("update@test.com")
            .WithFirstName("Original")
            .WithLastName("Name")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(userToUpdate));

        // Create updated version of the user
        var updatedUser = new UserBuilder()
            .WithId(userToUpdate.Id)
            .WithOrganizationId(organization.Id)
            .WithEmail(userToUpdate.Email)
            .WithFirstName("Updated")
            .WithLastName("UpdatedName")
            .WithPasswordHash(userToUpdate.PasswordHash)
            .WithIsActive(userToUpdate.IsActive)
            .Build();

        // Act
        var result = await _sut.UpdateAsync(updatedUser, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var retrievedUser = await _sut.GetByIdAsync(userToUpdate.Id, CancellationToken.None);
        retrievedUser.Should().NotBeNull();
        retrievedUser!.FirstName.Should().Be("Updated");
        retrievedUser.LastName.Should().Be("UpdatedName");
        retrievedUser.Email.Should().Be(userToUpdate.Email);
        retrievedUser.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.UpdateAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser_WhenValidUserProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var userToDelete = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("delete@test.com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(userToDelete));

        // Act
        var result = await _sut.DeleteAsync(userToDelete, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var deletedUser = await _sut.GetByIdAsync(userToDelete.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
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
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var tempUser = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("temp@test.com")
            .Build();

        // Act
        var result = await _sut.InsertAsync(tempUser, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        var insertedUser = await _sut.GetByIdAsync(tempUser.Id, CancellationToken.None);
        insertedUser.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithPersistFalse_ShouldNotPersistChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var userToUpdate = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("updatepersist@test.com")
            .WithFirstName("Original")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(userToUpdate));

        var updatedUser = new UserBuilder()
            .WithId(userToUpdate.Id)
            .WithOrganizationId(organization.Id)
            .WithEmail(userToUpdate.Email)
            .WithFirstName("TempUpdate")
            .WithPasswordHash(userToUpdate.PasswordHash)
            .WithIsActive(userToUpdate.IsActive)
            .Build();

        // Act
        var result = await _sut.UpdateAsync(updatedUser, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        // Verify the change wasn't persisted by checking the database
        var userFromDb = await _sut.GetByIdAsync(userToUpdate.Id, CancellationToken.None);
        userFromDb!.FirstName.Should().Be("Original");
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var pendingUser = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("pending@test.com")
            .Build();

        // Add entity without persisting
        await _sut.InsertAsync(pendingUser, false, CancellationToken.None);

        // Act
        var result = await _sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var savedUser = await _sut.GetByIdAsync(pendingUser.Id, CancellationToken.None);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("pending@test.com");
        savedUser.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task InsertOrUpdateAsync_ShouldInsertUser_WhenUserDoesNotExist()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var newUser = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("insertorupdate@test.com")
            .Build();

        // Act
        var result = await _sut.InsertOrUpdateAsync(u => u.Email == newUser.Email, newUser, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedUser = await _sut.GetByEmailAsync(newUser.Email, CancellationToken.None);
        insertedUser.Should().NotBeNull();
        insertedUser.Email.Should().Be("insertorupdate@test.com");
    }

    [Fact]
    public async Task InsertOrUpdateAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var existingUser = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("existinguser@test.com")
            .WithFirstName("Existing")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(existingUser));

        var updatedUser = new UserBuilder()
            .WithId(existingUser.Id)
            .WithOrganizationId(organization.Id)
            .WithEmail(existingUser.Email)
            .WithFirstName("UpdatedViaInsertOrUpdate")
            .WithPasswordHash(existingUser.PasswordHash)
            .WithIsActive(existingUser.IsActive)
            .Build();

        // Act
        var result = await _sut.InsertOrUpdateAsync(u => u.Email == existingUser.Email, updatedUser, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var retrievedUser = await _sut.GetByEmailAsync(existingUser.Email, CancellationToken.None);
        retrievedUser.Should().NotBeNull();
        retrievedUser.FirstName.Should().Be("UpdatedViaInsertOrUpdate");
    }

    #endregion
}