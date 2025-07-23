using TimeTracker.WebApi.Helpers;

namespace TimeTracker.WebApi.Middlewares;

public class BuildTimeHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public BuildTimeHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("x-build-date", VersionHelper.GetLinkerTime().ToString("yyyy-MM-dd HH:mm"));

        await _next(context);
    }
}