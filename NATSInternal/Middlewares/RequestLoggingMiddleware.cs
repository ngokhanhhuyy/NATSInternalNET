using System.Globalization;

namespace NATSInternal.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        string method = context.Request.Method;
        string path = context.Request.Path;
        QueryString queryString = context.Request.QueryString;
        int statusCode = context.Response.StatusCode;
        switch (statusCode)
        {
            case >= 200 and < 300:
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case >= 300 and < 400:
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case >= 400 and < 500:
                Console.BackgroundColor = ConsoleColor.Red;
                break;
            case >= 500:
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                break;
        }
        Console.Write(statusCode);
        Console.ResetColor();
        await Console.Out.WriteAsync("     ");
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(DateTime.UtcNow.ToApplicationTime().ToString(CultureInfo.InvariantCulture));
        Console.ResetColor();
        Console.WriteLine($" {method} {path}{queryString}");
    }
}