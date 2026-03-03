using FluentValidation;
using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Messaging;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Reflection.Emit;
using TimeTracker.Application.Behaviours;
using TimeTracker.Application.Behaviours.Authorization;
using TimeTracker.Application.Behaviours.Validation;
using TimeTracker.Application.Interfaces.Auth;

namespace TimeTracker.Application.Configuration;

public static class ServicesConfigExtensions
{
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        services.AddLiteBus(config =>
        {
            config.AddMessageModule(module =>
            {
                module.Register(typeof(GlobalLoggingHandler<>));
                module.Register(typeof(ValidationHandler<>));
                module.Register(typeof(AuthorizationHandler<>));
            });
            config.AddCommandModule(module => module.RegisterFromAssembly(Assembly.GetExecutingAssembly()));
            config.AddQueryModule(module => module.RegisterFromAssembly(Assembly.GetExecutingAssembly()));
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
