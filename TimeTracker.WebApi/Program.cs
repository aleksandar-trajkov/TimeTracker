using AttributeBuilder.IoC;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TimeTracker.WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
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
builder.Services.AddApplicationLogic();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "me";
        options.RequireHttpsMetadata = false;
    });

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddAuthorization();
builder.Services.AddMapster();
builder.Services.AddEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (bool.TryParse(builder.Configuration["Database:AutoMigrate"], out var autoMigrate) && autoMigrate)
{
    app.Services.MigrateDatabase();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapEndpoints(app.Services);

app.Run();