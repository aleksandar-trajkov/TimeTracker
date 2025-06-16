using FluentAssertions;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.Application.UseCases.Auth.Validators;
using TimeTracker.UnitTests.Application.UseCases.Validators.Auth.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Auth;

public class IsSignInRequestValidTests
{
    private readonly UserRepositoryMockDouble _userRepository;

    private readonly IsSignInRequestValid _validator;

    public IsSignInRequestValidTests()
    {
        _userRepository = new UserRepositoryMockDouble();

        _validator = new IsSignInRequestValid(_userRepository.Instance);
    }

    [Fact]
    public async Task ValidQuery_ShouldPassValidation()
    {
        var user = new UserBuilder().WithEmail("user@valid.com").Build();
        _userRepository.GivenExistsByEmail(user.Email, true);
        var query = new SignIn.Query(user.Email, user.PasswordHash);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(IsSignInRequestValidTheoryData))]
    public async Task InvalidQuery_ShouldFailValidation(SignIn.Query query, string expectedErrorMessage)
    {
        // Arrange
        _userRepository.GivenExistsByEmail(query.Email, false);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == expectedErrorMessage);
    }
}
