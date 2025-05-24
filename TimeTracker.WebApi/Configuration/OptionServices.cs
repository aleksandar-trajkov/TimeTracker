using Microsoft.Extensions.Options;
using System.Reflection;
using System.Runtime.CompilerServices;
using TimeTracker.Application.Extensions;
using TimeTracker.Domain.Options;
using TimeTracker.Domain.Options.Base;

namespace TimeTracker.WebApi.Configuration;

public static class OptionServices
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.Section));
        return services;
    }
}