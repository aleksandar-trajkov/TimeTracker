using FluentAssertions;
using FluentValidation;
using TimeTracker.Application.Behaviours;
using TimeTracker.Application.Extensions;
using TimeTracker.Application.UseCases.Auth.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;
using TimeTracker.UnitTests.Common.Builders.Domain.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Auth;

public class SignInTests
{
    private readonly UserRepositoryMockDouble _userRepository;

    private SignIn _sut;

    public SignInTests()
    {
        _userRepository = new UserRepositoryMockDouble();

        _sut = new SignIn(_userRepository.Instance);
    }

    [Fact]
    public async Task WhenCalling_SignIn_With_ValidCredentials_Returns_User()
    {
        // Arrange
        var email = "user@valid.com";
        var password = "validPassword";
        var user = new UserBuilder()
            .WithEmail(email)
            .WithPasswordHash(password.GenerateHash())
            .Build();

        _userRepository.GivenGetByEmail(email, user);

        var query = new SignIn.Query(email, "validPassword");

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<User>();
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task WhenCalling_SignIn_With_UserInvalidPassword_Throws_AuthenticationException()
    {
        // Arrange
        var email = "user@inactive.com";
        var password = "invalidPassword";
        var user = new UserBuilder()
            .WithEmail(email)
            .WithPasswordHash(password)
            .Build();

        _userRepository.GivenGetByEmail(email, user);

        var query = new SignIn.Query(email, password);

        // Act
        var result = await Record.ExceptionAsync(() => _sut.Handle(query, CancellationToken.None));

        // Assert
        result.Should().NotBeNull();
        var authenticationException = result.Should().BeOfType<AuthenticationException>().Subject;
        authenticationException.Message.Should().Be("Invalid username or password");
        authenticationException.Email.Should().Be(email);
    }

    [Fact]
    public async Task WhenCalling_SignIn_With_InactiveUser_Throws_AuthenticationException()
    {
        // Arrange
        var email = "user@inactive.com";
        var password = "validPassword";
        var user = new UserBuilder()
            .WithEmail(email)
            .WithPasswordHash(password.GenerateHash())
            .WithIsActive(false)
            .Build();

        _userRepository.GivenGetByEmail(email, user);

        var query = new SignIn.Query(email, password);

        // Act
        var result = await Record.ExceptionAsync(() => _sut.Handle(query, CancellationToken.None));

        // Assert
        result.Should().NotBeNull();
        var authenticationException = result.Should().BeOfType<AuthenticationException>().Subject;
        authenticationException.Message.Should().Be("User is not active in system");
        authenticationException.Email.Should().Be(email);
    }
}
