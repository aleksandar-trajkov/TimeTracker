using Asp.Versioning;
using Asp.Versioning.Builder;

namespace TimeTracker.WebApi.Configuration;

public static class ApiVersioningServices
{
    public static IServiceCollection AddApiVersions(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
        return services;
    }

    public static WebApplication UseApiVersioning(this WebApplication app, out RouteGroupBuilder groupBuilder)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        groupBuilder = app
            .MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        return app;
    }
}