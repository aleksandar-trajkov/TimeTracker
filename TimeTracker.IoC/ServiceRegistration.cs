using TimeTracker.Infrastructure.Data.SqlServer.Configuration;
using TimeTracker.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AttributeBuilder.IoC;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationLogic(this IServiceCollection services)
    {
        services.AddMediatorServices().AddValidators();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseServices(configuration);

        return services;
    }
}

