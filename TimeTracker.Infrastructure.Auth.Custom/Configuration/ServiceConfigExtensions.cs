using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Infrastructure.Auth.Custom.Authentication;

namespace TimeTracker.Infrastructure.Auth.Custom.Configuration;

public static class ServiceConfigExtensions
{
    public static IServiceCollection AddCustomAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenProvider, TokenProvider>();
        return services;
    }
}