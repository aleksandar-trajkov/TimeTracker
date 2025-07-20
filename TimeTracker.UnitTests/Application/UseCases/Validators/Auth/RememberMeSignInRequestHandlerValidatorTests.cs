using FluentAssertions;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.Application.UseCases.Auth.Validators;
using TimeTracker.UnitTests.Application.UseCases.Validators.Auth.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Application.Auth;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Auth;

public class RememberMeSignInRequestHandlerValidatorTests
{
    private readonly UserRepositoryMockDouble _userRepository;
    private readonly TokenProviderMockDouble _tokenProvider;
    private readonly RememberMeSignInRequestHandlerValidator _validator;

    public RememberMeSignInRequestHandlerValidatorTests()
    {
        _userRepository = new UserRepositoryMockDouble();
        _tokenProvider = new TokenProviderMockDouble();
        _validator = new RememberMeSignInRequestHandlerValidator(_tokenProvider.Instance, _userRepository.Instance);
    }

    [Fact]
    public async Task ValidQuery_ShouldPassValidation()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var validToken = $"remember_me_{user.Email}";
        
        var query = new RememberMeSignInQueryBuilder()
            .WithRememberMeToken(validToken)
            .Build();

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(IsRememberMeSignInRequestInvalidTheoryData))]
    public async Task InvalidQuery_ShouldFailValidation(RememberMeSignInRequestHandler.Query query, string expectedErrorMessage)
    {
        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == expectedErrorMessage);
    }
}