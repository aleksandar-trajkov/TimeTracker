using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeTracker.Application.Extensions;
using TimeTracker.Application.Helpers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Options;

namespace TimeTracker.WebApi.Helpers;

public static class AuthHelper
{
    public static string CreateToken(User user, AuthOptions authOptions)
    {
        var claims = new List<Claim>
        {
            new Claim("authority", authOptions.Authority),
            new Claim("email", user.Email),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName ?? string.Empty)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SecurityKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            issuer: authOptions.Authority,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(authOptions.ExpirationHours),
            signingCredentials: cred
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    public static string CreateRememberMeToken(string username, bool rememberMeToken, AuthOptions authOptions)
    {
        if (!rememberMeToken)
        {
            return null!;
        }
        var userId = EncryptionHelper.Constants.Fluff + username.ToString() + EncryptionHelper.Constants.Fluff;
        return EncryptionHelper.Encrypt(userId, authOptions.UserKey);
    }

    public static string? TryLoginEmailFromRememberMeToken(string? rememberMeToken, AuthOptions authOptions)
    {
        if (rememberMeToken != null)
        {
            var decryptedString = EncryptionHelper.Decrypt(rememberMeToken, authOptions.UserKey);
            return decryptedString.TrimStart(EncryptionHelper.Constants.Fluff).TrimEnd(EncryptionHelper.Constants.Fluff);
        }
        return null;
    }
}

