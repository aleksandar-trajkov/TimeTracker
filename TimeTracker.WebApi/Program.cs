using AttributeBuilder.IoC;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TimeTracker.Common.Configuration;
using TimeTracker.WebApi.Configuration;
using TimeTracker.WebApi.Helpers;
using TimeTracker.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.AddApiVersions();
builder.Services.AddCommonServices();
builder.Services.AddApplicationLogic();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);

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
RouteGroupBuilder groupBuilder;
app.UseHttpsRedirection();
app.UseApiVersioning(out groupBuilder);
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseMiddleware<BuildTimeHeaderMiddleware>();
groupBuilder.MapEndpoints(app.Services);

app.Run();