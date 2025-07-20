using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Reflection.Metadata.Ecma335;
using TimeTracker.Application.Behaviours;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Auth.Dtos;
using TimeTracker.Common.Encryption;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;

namespace TimeTracker.Application.UseCases.Auth.Handlers;

public class SignInRequestHandler : IRequestHandler<SignInRequestHandler.Query, SignInResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly EncryptionProvider _encryptionProvider;

    public SignInRequestHandler(IUserRepository userRepository, ITokenProvider tokenProvider, EncryptionProvider encryptionProvider)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _encryptionProvider = encryptionProvider;
    }
    public async Task<SignInResponseDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        var passwordHash = _encryptionProvider.GenerateHash(request.Password);
        if (!user.PasswordHash.Equals(passwordHash, StringComparison.InvariantCulture))
        {
            throw new AuthenticationException("Invalid username or password", request.Email);
        }

        if(!user.IsActive)
        {
            throw new AuthenticationException("User is not active in system", request.Email);
        }

        return new SignInResponseDto
        {
            Token = _tokenProvider.CreateAuthToken(user),
            RememberMeToken = _tokenProvider.CreateRememberMeToken(user.Email),
        };
    }

    public record Query(string Email, string Password) : IRequest<SignInResponseDto>;
}
