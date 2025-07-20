using FluentAssertions;
using NSubstitute;
using TimeTracker.Application.UseCases.Auth.Dtos;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;
using TimeTracker.UnitTests.Common.Builders.Application.Auth;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Auth;

public class RememberMeSignInRequestHandlerTests
{
    private readonly UserRepositoryMockDouble _userRepository;
    private readonly TokenProviderMockDouble _tokenProvider;
    private readonly RememberMeSignInRequestHandler _sut;

    public RememberMeSignInRequestHandlerTests()
    {
        _userRepository = new UserRepositoryMockDouble();
        _tokenProvider = new TokenProviderMockDouble();
        _sut = new RememberMeSignInRequestHandler(_userRepository.Instance, _tokenProvider.Instance);
    }

    [Fact]
    public async Task RememberMeSignIn_With_ValidToken_Returns_Tokens()
    {
        // Arrange
        var email = "user@valid.com";
        var rememberMeToken = "valid_remember_me_token";
        var user = new UserBuilder()
            .WithEmail(email)
            .WithIsActive(true)
            .Build();

        _tokenProvider.GivenTryGetLoginEmailFromRememberMeToken(rememberMeToken, email);
        _userRepository.GivenExistsByEmail(email, true);
        _userRepository.GivenGetByEmail(email, user);
        _tokenProvider.GivenGenerateToken(user, "validAuthToken");
        _tokenProvider.GivenGenerateRefreshToken(email, "newRememberMeToken");

        var query = new RememberMeSignInQueryBuilder()
            .WithRememberMeToken(rememberMeToken)
            .Build();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<SignInResponseDto>();
        result.Token.Should().Be("validAuthToken");
        result.RememberMeToken.Should().Be("newRememberMeToken");
    }

    [Fact]
    public async Task RememberMeSignIn_With_InvalidToken_Throws_AuthenticationException()
    {
        // Arrange
        var rememberMeToken = "invalid_remember_me_token";
        
        _tokenProvider.GivenTryGetLoginEmailFromRememberMeToken(rememberMeToken, null);

        var query = new RememberMeSignInQueryBuilder()
            .WithRememberMeToken(rememberMeToken)
            .Build();

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid remember me token");
        exception.Which.Email.Should().BeNull();
    }

    [Fact]
    public async Task RememberMeSignIn_With_EmptyEmailFromToken_Throws_AuthenticationException()
    {
        // Arrange
        var rememberMeToken = "token_with_empty_email";
        
        _tokenProvider.GivenTryGetLoginEmailFromRememberMeToken(rememberMeToken, string.Empty);

        var query = new RememberMeSignInQueryBuilder()
            .WithRememberMeToken(rememberMeToken)
            .Build();

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid remember me token");
        exception.Which.Email.Should().Be(string.Empty);
    }

    [Fact]
    public async Task RememberMeSignIn_With_NonExistentUser_Throws_AuthenticationException()
    {
        // Arrange
        var email = "nonexistent@user.com";
        var rememberMeToken = "valid_remember_me_token";

        _tokenProvider.GivenTryGetLoginEmailFromRememberMeToken(rememberMeToken, email);
        _userRepository.GivenExistsByEmail(email, false);

        var query = new RememberMeSignInQueryBuilder()
            .WithRememberMeToken(rememberMeToken)
            .Build();

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid remember me token");
    }

    [Fact]
    public async Task RememberMeSignIn_With_InactiveUser_Throws_AuthenticationException()
    {
        // Arrange
        var email = "inactive@user.com";
        var rememberMeToken = "valid_remember_me_token";
        var user = new UserBuilder()
            .WithEmail(email)
            .WithIsActive(false)
            .Build();

        _tokenProvider.GivenTryGetLoginEmailFromRememberMeToken(rememberMeToken, email);
        _userRepository.GivenExistsByEmail(email, true);
        _userRepository.GivenGetByEmail(email, user);

        var query = new RememberMeSignInQueryBuilder()
            .WithRememberMeToken(rememberMeToken)
            .Build();

        // Act
        var act = async () => await _sut.Handle(query, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("User is not active in system");
        exception.Which.Email.Should().Be(email);
    }
}