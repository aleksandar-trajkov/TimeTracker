using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TimeTracker.WebApi.Options;

namespace TimeTracker.WebApi.Configuration;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Options.HealthCheckOptions>(configuration.GetSection("HealthCheck"));
        services
            .AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("Database")!, name: "SqlServer");

        return services;
    }

    public static WebApplication UseHealthCheck(this WebApplication app, IConfiguration configuration)
    {
        var healthCheckOptions = configuration.GetSection("HealthCheck").Get<Options.HealthCheckOptions>()!;
        app.MapHealthChecks("/api" + healthCheckOptions.Endpoint, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
