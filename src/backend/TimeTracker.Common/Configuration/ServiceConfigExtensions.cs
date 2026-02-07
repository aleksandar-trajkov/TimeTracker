using Microsoft.Extensions.DependencyInjection;
using TimeTracker.Application.Interfaces.Common;
using TimeTracker.Common.Date;
using TimeTracker.Common.Encryption;

namespace TimeTracker.Common.Configuration;

public static class ServiceConfigExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<EncryptionProvider>();
        return services;
    }
}