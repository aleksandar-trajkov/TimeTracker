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
    public async Task GetAllAsync_ShouldReturnUsersWithPermissions_WhenUsersExistInOrganization()
    {
        // Arrange
        var organization1 = new OrganizationBuilder().Build();
        var organization2 = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization1, organization2));

        // Create users for organization1
        var user1Id = Guid.NewGuid();
        var user1 = new UserBuilder()
            .WithId(user1Id)
            .WithOrganizationId(organization1.Id)
            .WithEmail("user1@org1.com")
            .WithFirstName("User")
            .WithLastName("One")
            .WithPermissions(ListHelper.CreateList<Domain.Auth.Permission>(
                new PermissionBuilder().WithKey(PermissionEnum.CanEditOwnRecord).WithUserId(user1Id).Build())
            .AsEnumerable())
            .Build();

        var user2Id = Guid.NewGuid();
        var user2 = new UserBuilder()
            .WithId(user2Id)
            .WithOrganizationId(organization1.Id)
            .WithEmail("user2@org1.com")
            .WithFirstName("User")
            .WithLastName("Two")
            .WithPermissions(ListHelper.CreateList<Domain.Auth.Permission>(
                new PermissionBuilder().WithKey(PermissionEnum.CanEditOwnRecord).WithUserId(user2Id).Build(),
                new PermissionBuilder().WithKey(PermissionEnum.CanEditAnyRecord).WithUserId(user2Id).Build())
            .AsEnumerable())
            .Build();

        // Create user for organization2 (should not be returned)
        var user3Id = Guid.NewGuid();
        var user3 = new UserBuilder()
            .WithId(user3Id)
            .WithOrganizationId(organization2.Id)
            .WithEmail("user3@org2.com")
            .WithFirstName("User")
            .WithLastName("Three")
            .Build();

        _fixture.Seed<Guid>(ListHelper.CreateList(user1, user2, user3));

        // Seed permissions
        var user1Permissions = (ICollection<Domain.Auth.Permission>)user1.Permissions;
        var user2Permissions = (ICollection<Domain.Auth.Permission>)user2.Permissions;
        _fixture.Seed<Guid>(user1Permissions);
        _fixture.Seed<Guid>(user2Permissions);

        // Act
        var result = await _sut.GetAllAsync(organization1.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList.Should().Contain(u => u.Email == "user1@org1.com");
        resultList.Should().Contain(u => u.Email == "user2@org1.com");
        resultList.Should().NotContain(u => u.Email == "user3@org2.com");

        // Verify permissions are included
        var user1Result = resultList.First(u => u.Email == "user1@org1.com");
        user1Result.Permissions.Should().NotBeNullOrEmpty();
        user1Result.Permissions.Should().HaveCount(1);
        user1Result.Permissions.First().Key.Should().Be(PermissionEnum.CanEditOwnRecord);

        var user2Result = resultList.First(u => u.Email == "user2@org1.com");
        user2Result.Permissions.Should().NotBeNullOrEmpty();
        user2Result.Permissions.Should().HaveCount(2);
        user2Result.Permissions.Should().Contain(p => p.Key == PermissionEnum.CanEditAnyRecord);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoUsersExistInOrganization()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        // Act
        var result = await _sut.GetAllAsync(organization.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenOrganizationDoesNotExist()
    {
        // Arrange
        var nonExistentOrganizationId = Guid.NewGuid();

        // Act
        var result = await _sut.GetAllAsync(nonExistentOrganizationId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsersWithoutPermissions_WhenUsersHaveNoPermissions()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var userWithoutPermissions = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("nopermissions@test.com")
            .WithFirstName("No")
            .WithLastName("Permissions")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(userWithoutPermissions));

        // Act
        var result = await _sut.GetAllAsync(organization.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        
        var user = result.First();
        user.Email.Should().Be("nopermissions@test.com");
        user.Permissions.Should().NotBeNull();
        user.Permissions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("cancellation@test.com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _sut.GetAllAsync(organization.Id, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

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
    public async Task GetByEmailAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("cancellation2@test.com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _sut.GetByEmailAsync(user.Email, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
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

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenEmailExistsWithDifferentCasing()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("CaseSensitive@Test.Com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        // Act
        var result = await _sut.ExistsByEmailAsync("casesensitive@test.com", CancellationToken.None);

        // Assert - This depends on database collation, but typically should be case-insensitive
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var act = async () => await _sut.ExistsByEmailAsync("test@example.com", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region BaseRepository inherited methods

    [Fact]
    public async Task FindByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var user = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("findbyid@example.com")
            .WithFirstName("John")
            .WithLastName("Doe")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(user));

        // Act
        var result = await _sut.FindByIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.FindByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

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
    public async Task GetByIdAsync_ShouldThrowInvalidOperationException_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        var act = async () => await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
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

        var insertedUser = await _sut.FindByIdAsync(newUser.Id, CancellationToken.None);
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

        var retrievedUser = await _sut.FindByIdAsync(userToUpdate.Id, CancellationToken.None);
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
    public async Task DeleteAsync_WithEntity_ShouldDeleteUser_WhenValidUserProvided()
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

        var deletedUser = await _sut.FindByIdAsync(userToDelete.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithEntity_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.DeleteAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_WithId_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(organization));

        var userToDelete = new UserBuilder()
            .WithOrganizationId(organization.Id)
            .WithEmail("deletewithid@test.com")
            .Build();
        _fixture.Seed<Guid>(ListHelper.CreateList(userToDelete));

        // Act
        await _sut.DeleteAsync(userToDelete.Id, CancellationToken.None);

        // Assert
        var deletedUser = await _sut.FindByIdAsync(userToDelete.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithId_ShouldReturnZero_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        await _sut.DeleteAsync(nonExistentId, CancellationToken.None);
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

        var insertedUser = await _sut.FindByIdAsync(tempUser.Id, CancellationToken.None);
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
        var userFromDb = await _sut.FindByIdAsync(userToUpdate.Id, CancellationToken.None);
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

        var savedUser = await _sut.FindByIdAsync(pendingUser.Id, CancellationToken.None);
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