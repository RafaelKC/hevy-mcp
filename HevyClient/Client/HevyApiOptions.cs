namespace HevyClient.Client;

public sealed class HevyApiOptions
{
    /// <summary>
    /// API base URL. Your endpoints are paths like "/v1/workouts", so the base is usually "https://hevy.com".
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.hevyapp.com";
}

