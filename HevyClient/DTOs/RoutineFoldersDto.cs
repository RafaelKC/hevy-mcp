using System.Text.Json.Serialization;

namespace HevyClient.DTOs;

public sealed class PaginatedRoutineFoldersResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("routine_folders")]
    public List<RoutineFolder> RoutineFolders { get; init; } = new();
}

public sealed class RoutineFolder
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; init; } = "";

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; init; } = "";
}

// ----------------------------
// Requests - Routine folders
// ----------------------------

public sealed class PostRoutineFolderRequestBody
{
    [JsonPropertyName("routine_folder")]
    public PostRoutineFolderRequest RoutineFolder { get; init; } = new();
}

public sealed class PostRoutineFolderRequest
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = "";
}

