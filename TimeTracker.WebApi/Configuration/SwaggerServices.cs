using Asp.Versioning;
using Microsoft.OpenApi.Models;

namespace TimeTracker.WebApi.Configuration
{
    public static class SwaggerServices
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Charge point management service", Version = "v1" });
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securitySchema,
                        new string[] { "Bearer" }
                    }
                });
            });
            
            return services;
        }
    }
}