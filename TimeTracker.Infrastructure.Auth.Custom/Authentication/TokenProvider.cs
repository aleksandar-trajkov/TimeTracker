using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Common.Encryption;
using TimeTracker.Common.Extensions;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Options;

namespace TimeTracker.Infrastructure.Auth.Custom.Authentication;

internal sealed class TokenProvider : ITokenProvider
{
    public const string Fluff = "mnygrtjnw4e";
    private readonly EncryptionProvider _encryptionProvider;
    private readonly AuthOptions _authOptions;

    public TokenProvider(
        EncryptionProvider encryptionProvider,
        IOptions<AuthOptions> options)
    {
        _authOptions = options.Value;
        _encryptionProvider = encryptionProvider;
    }

    public string CreateAuthToken(User user)
    {
        string secretKey = _authOptions.SecurityKey!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName ?? string.Empty),
                new Claim("organizationId", user.OrganizationId.ToString()),
            ]),
            Expires = DateTime.UtcNow.AddHours(_authOptions.ExpirationHours),
            SigningCredentials = credentials,
            Issuer = _authOptions.Authority,
            Audience = _authOptions.Audience,

        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public string? CreateRememberMeToken(string username)
    {
        var userId = Fluff + username.ToString() + Fluff;
        return _encryptionProvider.Encrypt(userId, _authOptions.UserKey);
    }

    public string? TryGetLoginEmailFromRememberMeToken(string? rememberMeToken)
    {
        if (rememberMeToken != null)
        {
            var decryptedString = _encryptionProvider.Encrypt(rememberMeToken, _authOptions.UserKey);
            return decryptedString.TrimStart(Fluff).TrimEnd(Fluff);
        }
        return null;
    }
}
