using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TimeTracker.Application.Behaviours;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data.Base;
using TimeTracker.Application.UseCases.TimeEntries.Authorizators;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.Application.Configuration;

public static class ServicesConfigExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddAuthorizators(this IServiceCollection services)
    {
        var typesToRegister = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => !string.IsNullOrEmpty(type.Namespace) && type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAuthorizator<>)));

        foreach (var type in typesToRegister)
        {
            var interfaceType = type.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAuthorizator<>));
            services.AddScoped(interfaceType, type);
        }

        return services;
    }
}
