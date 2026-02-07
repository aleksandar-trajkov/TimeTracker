using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Domain.Options;
using TimeTracker.Infrastructure.Auth.Custom.Authentication;

namespace TimeTracker.Infrastructure.Auth.Custom.Configuration;

public static class ServiceConfigExtensions
{
    public static IServiceCollection AddCustomAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var authOptions = configuration.GetSection(AuthOptions.Section).Get<AuthOptions>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SecurityKey)),
                    ValidIssuer = authOptions.Authority,
                    ValidAudience = authOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        return services;
    }
}