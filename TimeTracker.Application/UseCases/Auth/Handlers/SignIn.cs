using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TimeTracker.Application.Behaviours;
using TimeTracker.Application.Helpers;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;

namespace TimeTracker.Application.UseCases.Auth.Handlers;

public class SignIn : IRequestHandler<SignIn.Query, User>
{
    private readonly IUserRepository _userRepository;

    public SignIn(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<User> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        var passwordHash = EncryptionHelper.GenerateHash(request.Password);
        if (!user.PasswordHash.Equals(passwordHash, StringComparison.InvariantCulture))
        {
            throw new AuthenticationException("Invalid username or password", request.Email);
        }

        if(!user.IsActive)
        {
            throw new AuthenticationException("User is not active in system", request.Email);
        }

        return user;
    }

    public record Query(string Email, string Password) : IRequest<User>;
}
