using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TimeTracker.WebApi.Interfaces;

namespace TimeTracker.WebApi.Configuration;

public static class MinimalApiEndpointServices
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var typesToRegister = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(p => p.IsClass && typeof(IEndpointDefinition).IsAssignableFrom(p));
        foreach (var type in typesToRegister)
        {
            services.AddTransient(type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app, IServiceProvider serviceProvider)
    {
        var typesToRegister = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(p => p.IsClass && typeof(IEndpointDefinition).IsAssignableFrom(p));

        foreach (var type in typesToRegister)
        {
            var instance = serviceProvider.GetService(type) as IEndpointDefinition;
            instance?.Map(app);
        }

        return app;
    }
}
