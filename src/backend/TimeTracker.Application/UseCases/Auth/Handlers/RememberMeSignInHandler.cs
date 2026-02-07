using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Auth.Dtos;
using TimeTracker.Domain.Exceptions;

namespace TimeTracker.Application.UseCases.Auth.Handlers;

public class RememberMeSignInHandler : IRequestHandler<RememberMeSignInHandler.Query, SignInResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;

    public RememberMeSignInHandler(IUserRepository userRepository, ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
    }

    public async Task<SignInResponseDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var email = _tokenProvider.TryGetLoginEmailFromRememberMeToken(request.RememberMeToken);

        if (string.IsNullOrEmpty(email))
        {
            throw new AuthenticationException("Invalid remember me token", null!);
        }

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken) == false)
        {
            throw new AuthenticationException("Invalid remember me token", email);
        }

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (!user.IsActive)
        {
            throw new AuthenticationException("User is not active in system", email);
        }

        return new SignInResponseDto
        {
            Token = _tokenProvider.CreateAuthToken(user),
            RememberMeToken = _tokenProvider.CreateRememberMeToken(user.Email),
        };
    }

    public record Query(string RememberMeToken) : IRequest<SignInResponseDto>;
}