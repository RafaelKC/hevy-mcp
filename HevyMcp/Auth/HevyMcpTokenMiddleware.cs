namespace HevyMcp.Auth;

public sealed class HevyMcpTokenMiddleware
{
    public const string HttpContextItemApiKey = "HevyApiKey";
    public const string HttpContextItemBaseUrl = "HevyBaseUrl";

    private readonly RequestDelegate _next;
    private readonly HevyMcpTokenService _tokenService;

    public HevyMcpTokenMiddleware(RequestDelegate next, HevyMcpTokenService tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only protect MCP endpoint.
        if (!context.Request.Path.StartsWithSegments("/mcp"))
        {
            await _next(context);
            return;
        }

        var auth = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing Authorization: Bearer token.");
            return;
        }

        var token = auth.Substring("Bearer ".Length).Trim();

        try
        {
            var payload = _tokenService.ValidateAndDecode(token, DateTimeOffset.UtcNow);
            context.Items[HttpContextItemApiKey] = payload.HevyApiKey;
            if (!string.IsNullOrWhiteSpace(payload.HevyBaseUrl))
                context.Items[HttpContextItemBaseUrl] = payload.HevyBaseUrl;

            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(ex.Message);
        }
    }
}

