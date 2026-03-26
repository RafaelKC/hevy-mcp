using HevyMcp.Auth;

// ---- CLI commands (no server) ----
// dotnet run -- issue-token --hevy-api-key <uuid> [--ttl-minutes 60] [--base-url https://api.hevyapp.com]
// dotnet run -- generate-master-key
if (args.Length > 0)
{
    var command = args[0];
    if (string.Equals(command, "generate-master-key", StringComparison.OrdinalIgnoreCase))
    {
        var key = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(key);
        Console.WriteLine(Convert.ToBase64String(key));
        return;
    }

    if (string.Equals(command, "issue-token", StringComparison.OrdinalIgnoreCase))
    {
        var authOptions = new HevyMcpAuthOptions
        {
            MasterKeyBase64 = Environment.GetEnvironmentVariable("HEVY_MCP_MASTER_KEY_BASE64") ?? "",
            TokenTtlMinutes = int.TryParse(Environment.GetEnvironmentVariable("HEVY_MCP_TOKEN_TTL_MINUTES"), out var cliTtlMinutes) ? cliTtlMinutes : 60
        };
        var tokenSvc = new HevyMcpTokenService(authOptions);

        string? hevyApiKey = null;
        string? baseUrl = null;
        int? ttlMinutes = null;

        for (var i = 1; i < args.Length; i++)
        {
            var a = args[i];
            if (a == "--hevy-api-key" && i + 1 < args.Length) hevyApiKey = args[++i];
            else if (a == "--base-url" && i + 1 < args.Length) baseUrl = args[++i];
            else if (a == "--ttl-minutes" && i + 1 < args.Length && int.TryParse(args[++i], out var m)) ttlMinutes = m;
        }

        hevyApiKey ??= Environment.GetEnvironmentVariable("HEVY_API_KEY");
        baseUrl ??= Environment.GetEnvironmentVariable("HEVY_BASE_URL");

        if (string.IsNullOrWhiteSpace(hevyApiKey))
        {
            Console.Error.WriteLine("Missing Hevy API key. Pass --hevy-api-key or set HEVY_API_KEY.");
            Environment.ExitCode = 2;
            return;
        }

        var ttl = TimeSpan.FromMinutes(ttlMinutes ?? authOptions.TokenTtlMinutes);
        var token = tokenSvc.IssueToken(hevyApiKey, baseUrl, DateTimeOffset.UtcNow, ttl);
        Console.WriteLine(token);
        return;
    }
}

// ---- HTTP MCP server ----
var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Configuration.Sources.Clear();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpContextAccessor();

var auth = new HevyMcpAuthOptions
{
    MasterKeyBase64 = builder.Configuration["HEVY_MCP_MASTER_KEY_BASE64"] ?? "",
    TokenTtlMinutes = int.TryParse(builder.Configuration["HEVY_MCP_TOKEN_TTL_MINUTES"], out var serverTtlMinutes) ? serverTtlMinutes : 60
};
builder.Services.AddSingleton(auth);
builder.Services.AddSingleton<HevyMcpTokenService>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.UseMiddleware<HevyMcpTokenMiddleware>();
app.MapMcp("/mcp");

app.Run("http://0.0.0.0:3001");