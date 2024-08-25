using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace API.Middleware;



public class OAuthStateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OAuthStateMiddleware> _logger;

    public OAuthStateMiddleware(RequestDelegate next, ILogger<OAuthStateMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/auth/google-login"))
        {
            var state = context.Request.Query["state"].ToString();

            if (string.IsNullOrEmpty(state))
            {
                _logger.LogWarning("OAuth state is missing or invalid.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("OAuth state is missing or invalid.");
                return;
            }
        }

        await _next(context);
    }
}
