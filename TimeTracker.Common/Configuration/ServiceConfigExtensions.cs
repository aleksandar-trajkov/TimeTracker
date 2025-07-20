using Microsoft.Extensions.DependencyInjection;
using TimeTracker.Common.Encryption;

namespace TimeTracker.Common.Configuration;

public static class ServiceConfigExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<EncryptionProvider>();
        return services;
    }
}