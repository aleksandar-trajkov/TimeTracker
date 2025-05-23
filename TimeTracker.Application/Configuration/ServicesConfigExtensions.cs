using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TimeTracker.Application.Behaviours;

namespace TimeTracker.Application.Configuration;

public static class ServicesConfigExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddMediatR(config => {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
