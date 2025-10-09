namespace MoviesWebApp;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("Before call of middleware in separate class");
        Console.WriteLine(context.Request.Path);
        context.Response.Headers.Append("Some-Header", "Test Header");
        await _next.Invoke(context);
        Console.WriteLine("After call of middleware in separate class");
    }
}

public static class RequestCultureMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomMiddleware>();
    }
}