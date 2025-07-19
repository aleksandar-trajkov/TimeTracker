using Microsoft.Extensions.DependencyInjection;
using TimeTracker.Application.Extensions;

namespace TimeTracker.Common.Configuration;

public static class ServiceConfigExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<EncryptionProvider>();
        return services;
    }
}