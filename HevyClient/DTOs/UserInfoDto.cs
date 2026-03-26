using System.Text.Json.Serialization;

namespace HevyClient.DTOs;

public sealed class UserInfoResponse
{
    [JsonPropertyName("data")]
    public UserInfo Data { get; init; } = new();
}

public sealed class UserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("name")]
    public string Name { get; init; } = "";

    [JsonPropertyName("url")]
    public string Url { get; init; } = "";
}

