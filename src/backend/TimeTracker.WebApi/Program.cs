using AttributeBuilder.IoC;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text.Json.Serialization;
using TimeTracker.Common.Configuration;
using TimeTracker.WebApi.Configuration;
using TimeTracker.WebApi.Helpers;
using TimeTracker.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services.AddCommonServices();
builder.Services.AddApplicationLogic();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddHealthCheck(builder.Configuration);
builder.Services.AddApiVersionServices();
builder.Services.ConfigureHttpJsonOptions(options =>
{
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
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddOpenApi();


var app = builder.Build();
RouteGroupBuilder groupBuilder;
app.UseExceptionHandler();
app.UseApiVersionServices(out groupBuilder);
if (bool.TryParse(builder.Configuration["Database:AutoMigrate"], out var autoMigrate) && autoMigrate)
{
    app.Services.MigrateDatabase();
}
app.UseHealthCheck(builder.Configuration);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseMiddleware<BuildTimeHeaderMiddleware>();
groupBuilder.MapEndpoints(app.Services);
app.MapOpenApi();

app.Run();