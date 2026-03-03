using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker.Application.Interfaces.Common;

namespace TimeTracker.Infrastructure.Caching.Memory.Configuration;

public static class ServicesConfigExtensions
{
    public static IServiceCollection AddMemoryCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICache, CustomMemoryCache>();

        return services;
    }
}