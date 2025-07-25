using Asp.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TimeTracker.WebApi.Filters;

namespace TimeTracker.WebApi.Configuration
{
    public static class SwaggerServices
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Using the Authorization header with the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,
                    }
                };
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Time Tracking API service", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Time Tracking API service", Version = "v2" });
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securitySchema,
                        new string[] { "Bearer" }
                    }
                });
                c.DocumentFilter<HealthCheckFilter>();
            });

            return services;
        }

        public static WebApplication UseSwaggerServices(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint($"/swagger/v1/swagger.json", "v1");
                    options.SwaggerEndpoint($"/swagger/v2/swagger.json", "v2");
                });
            }

            return app;
        }
    }
}