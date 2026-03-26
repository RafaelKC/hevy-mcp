# hevy-mcp

A C# [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) server using **stdio** transport, intended to connect the [Hevy](https://www.hevyapp.com/) app to clients such as Cursor.

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) compatible with the project `TargetFramework` (currently `net10.0`).

## Build and run locally

```bash
dotnet build
dotnet run
```

The process speaks MCP over **stdin/stdout**. Do not treat it as an HTTP service; MCP clients start the executable as a subprocess.

## Cursor

1. Add `.cursor/mcp.json` at the workspace root (or `~/.cursor/mcp.json` for a global setup).
2. Example using `dotnet run`:

```json
{
  "mcpServers": {
    "hevy-mcp": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/hevy-mcp.csproj",
        "--no-launch-profile"
      ]
    }
  }
}
```

Adjust the `.csproj` path if the folder you open in Cursor is not this repository root.

3. Reload the Cursor window (**Developer: Reload Window**) or toggle the server under **Settings → Features → Model Context Protocol** after code changes.

Logs: **View → Output → MCP Logs**.

## Tools

Tools are discovered from the assembly via `[McpServerToolType]` / `[McpServerTool]` (`ModelContextProtocol` SDK). There is currently a sample `Echo` tool; replace or extend it with Hevy API calls as needed.

## License

MIT — see [LICENSE](LICENSE).
