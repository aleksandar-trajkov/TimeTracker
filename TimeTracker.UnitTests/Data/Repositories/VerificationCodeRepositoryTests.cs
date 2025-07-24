using FluentAssertions;
using TimeTracker.Domain.Auth;
using TimeTracker.Infrastructure.Data.SqlServer.Repositories;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Data.Fixtures;

namespace TimeTracker.UnitTests.Data.Repositories;

public class VerificationCodeRepositoryTests : IClassFixture<DataTestFixture>
{
    private readonly VerificationCodeRepository _sut;
    private readonly DataTestFixture _fixture;

    public VerificationCodeRepositoryTests(DataTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new VerificationCodeRepository(_fixture.Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnVerificationCode_WhenVerificationCodeExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var verificationCode = new VerificationCodeBuilder()
            .WithUserId(user.Id)
            .WithCode("TEST123456")
            .AsValid()
            .Build();
        _fixture.Seed<Guid>(new[] { verificationCode });

        // Act
        var result = await _sut.GetByIdAsync(verificationCode.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(verificationCode.Id);
        result.Code.Should().Be("TEST123456");
        result.UserId.Should().Be(user.Id);
        result.IsUsed.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenVerificationCodeDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenVerificationCodeExists()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var verificationCode = new VerificationCodeBuilder().WithUserId(user.Id).Build();
        _fixture.Seed<Guid>(new[] { verificationCode });

        // Act
        var result = await _sut.ExistsAsync(verificationCode.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenVerificationCodeDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsAsync(nonExistentId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertVerificationCode_WhenValidVerificationCodeProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var verificationCode = new VerificationCodeBuilder()
            .WithUserId(user.Id)
            .WithCode("NEWCODE123")
            .AsValid()
            .Build();

        // Act
        var result = await _sut.InsertAsync(verificationCode, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var insertedVerificationCode = await _sut.GetByIdAsync(verificationCode.Id, CancellationToken.None);
        insertedVerificationCode.Should().NotBeNull();
        insertedVerificationCode!.Code.Should().Be("NEWCODE123");
        insertedVerificationCode.UserId.Should().Be(user.Id);
        insertedVerificationCode.IsUsed.Should().BeFalse();
    }

    [Fact]
    public async Task InsertAsync_ShouldThrowArgumentNullException_WhenVerificationCodeIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.InsertAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateVerificationCode_WhenValidVerificationCodeProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var verificationCode = new VerificationCodeBuilder()
            .WithUserId(user.Id)
            .AsValid()
            .Build();
        _fixture.Seed<Guid>(new[] { verificationCode });

        // Modify the verification code
        verificationCode.IsUsed = true;
        verificationCode.Code = "UPDATED123";

        // Act
        var result = await _sut.UpdateAsync(verificationCode, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var updatedVerificationCode = await _sut.GetByIdAsync(verificationCode.Id, CancellationToken.None);
        updatedVerificationCode.Should().NotBeNull();
        updatedVerificationCode!.IsUsed.Should().BeTrue();
        updatedVerificationCode.Code.Should().Be("UPDATED123");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenVerificationCodeIsNull()
    {
        // Act & Assert
        var act = async () => await _sut.UpdateAsync(null!, true, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteVerificationCode_WhenValidVerificationCodeProvided()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var verificationCode = new VerificationCodeBuilder().WithUserId(user.Id).Build();
        _fixture.Seed<Guid>(new[] { verificationCode });

        // Act
        var result = await _sut.DeleteAsync(verificationCode, true, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var deletedVerificationCode = await _sut.GetByIdAsync(verificationCode.Id, CancellationToken.None);
        deletedVerificationCode.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentNullException_WhenVerificationCodeIsNull()
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

        var verificationCode = new VerificationCodeBuilder().WithUserId(user.Id).Build();

        // Act
        var result = await _sut.InsertAsync(verificationCode, false, CancellationToken.None);

        // Assert
        result.Should().Be(0);

        var insertedVerificationCode = await _sut.GetByIdAsync(verificationCode.Id, CancellationToken.None);
        insertedVerificationCode.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingChanges()
    {
        // Arrange
        var organization = new OrganizationBuilder().Build();
        var user = new UserBuilder().WithOrganizationId(organization.Id).Build();
        
        _fixture.Seed<Guid>(new[] { organization });
        _fixture.Seed<Guid>(new[] { user });

        var verificationCode = new VerificationCodeBuilder().WithUserId(user.Id).Build();

        // Add entity without persisting
        await _sut.InsertAsync(verificationCode, false, CancellationToken.None);

        // Act
        var result = await _sut.SaveChangesAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        var savedVerificationCode = await _sut.GetByIdAsync(verificationCode.Id, CancellationToken.None);
        savedVerificationCode.Should().NotBeNull();
    }
}