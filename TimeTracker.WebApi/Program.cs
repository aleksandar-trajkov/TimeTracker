using AttributeBuilder.IoC;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;
using TimeTracker.Common.Configuration;
using TimeTracker.WebApi.Configuration;
using TimeTracker.WebApi.Helpers;
using TimeTracker.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCommonServices();
builder.Services.AddApplicationLogic();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddHealthCheck(builder.Configuration);
builder.Services.AddApiVersionServices();
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
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerServices();


var app = builder.Build();
RouteGroupBuilder groupBuilder;
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
app.UseSwaggerServices();

app.Run();