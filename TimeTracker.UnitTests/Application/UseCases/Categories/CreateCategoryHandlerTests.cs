using FluentAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Categories;

public class CreateCategoryHandlerTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly CreateCategoryHandler _sut;

    public CreateCategoryHandlerTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _sut = new CreateCategoryHandler(_categoryRepository.Instance);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCreateCategoryAndReturnId()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var command = new CreateCategoryHandler.Command("Test Category", "Test Description", organizationId);

        _categoryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        var cat = new CategoryBuilder()
            .WithName("Test Category")
            .WithDescription("Test Description")
            .WithOrganizationId(organizationId)
            .Build();
        await _categoryRepository.VerifyInsertAsyncWasCalledWith(cat, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenNoDescription_ShouldCreateCategoryWithNullDescription()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var command = new CreateCategoryHandler.Command("Test Category", null, organizationId);

        _categoryRepository.GivenInsertAsync();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
        var cat = new CategoryBuilder()
            .WithName("Test Category")
            .WithDescription(null)
            .WithOrganizationId(organizationId)
            .Build();
        await _categoryRepository.VerifyInsertAsyncWasCalledWith(cat, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassCancellationToken()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var command = new CreateCategoryHandler.Command("Test Category", "Test Description", organizationId);
        var cancellationToken = new CancellationToken(true);

        _categoryRepository.GivenInsertAsync(cancellationToken);

        // Act & Assert
        await _sut.Handle(command, cancellationToken);


        var cat = new CategoryBuilder()
            .WithName("Test Category")
            .WithDescription("Test Description")
            .WithOrganizationId(organizationId)
            .Build();
        await _categoryRepository.VerifyInsertAsyncWasCalledWith(cat, cancellationToken);
    }
}