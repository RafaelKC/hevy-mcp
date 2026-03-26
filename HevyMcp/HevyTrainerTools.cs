using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using HevyApiClient = HevyClient.Client.HevyClient;
using HevyClient.Client;
using HevyClient.DTOs;
using ModelContextProtocol.Server;

namespace HevyMcp;

[McpServerToolType]
public static class HevyTrainerTools
{
    private static readonly Lazy<HevyApiClient> HevyClient = new(CreateClient);

    private static HevyApiClient Client => HevyClient.Value;

    public sealed record HevyConfigStatus(
        string EnvironmentName,
        string? BaseUrl,
        bool HasApiKey,
        string ApiKeySource,
        string BaseUrlSource);

    [McpServerTool, Description("Diagnose Hevy configuration (env/appsettings). Does not reveal the API key.")]
    public static HevyConfigStatus GetHevyConfigStatus()
    {
        var envName =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
            "Production";

        var (apiKey, apiKeySource, baseUrl, baseUrlSource) = ReadHevyConfig(envName);

        return new HevyConfigStatus(
            EnvironmentName: envName,
            BaseUrl: string.IsNullOrWhiteSpace(baseUrl) ? null : baseUrl,
            HasApiKey: !string.IsNullOrWhiteSpace(apiKey),
            ApiKeySource: apiKeySource,
            BaseUrlSource: baseUrlSource);
    }

    private static HevyApiClient CreateClient()
    {
        var envName =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
            "Production";

        var (apiKey, _, baseUrl, _) = ReadHevyConfig(envName);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Missing Hevy API key. Set HEVY_API_KEY (or one of MCP_HEVY_API_KEY, HEVY_APIKEY, HEVY_TOKEN, API_KEY) or put Hevy:ApiKey in appsettings.json/appsettings.Development.json.");
        }

        var options = !string.IsNullOrWhiteSpace(baseUrl) ? new HevyApiOptions { BaseUrl = baseUrl } : null;

        return new HevyApiClient(apiKey, options);
    }

    private static (string? ApiKey, string ApiKeySource, string? BaseUrl, string BaseUrlSource) ReadHevyConfig(string envName)
    {
        string? apiKey = null;
        string apiKeySource = "none";

        string? baseUrl = null;
        string baseUrlSource = "default";

        apiKey = Environment.GetEnvironmentVariable("HEVY_API_KEY");
        if (!string.IsNullOrWhiteSpace(apiKey)) apiKeySource = "env:HEVY_API_KEY";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("MCP_HEVY_API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKey)) apiKeySource = "env:MCP_HEVY_API_KEY";
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("HEVY_APIKEY");
            if (!string.IsNullOrWhiteSpace(apiKey)) apiKeySource = "env:HEVY_APIKEY";
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("HEVY_TOKEN");
            if (!string.IsNullOrWhiteSpace(apiKey)) apiKeySource = "env:HEVY_TOKEN";
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKey)) apiKeySource = "env:API_KEY";
        }

        baseUrl = Environment.GetEnvironmentVariable("HEVY_BASE_URL");
        if (!string.IsNullOrWhiteSpace(baseUrl)) baseUrlSource = "env:HEVY_BASE_URL";

        // appsettings fallback (output folder)
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(baseUrl))
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: false)
                .Build();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = config["Hevy:ApiKey"];
                if (!string.IsNullOrWhiteSpace(apiKey)) apiKeySource = $"appsettings:{envName}";
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = config["Hevy:BaseUrl"];
                if (!string.IsNullOrWhiteSpace(baseUrl)) baseUrlSource = $"appsettings:{envName}";
            }
        }

        // Default base URL if nothing is configured anywhere
        baseUrl ??= "https://hevy.com";

        return (apiKey, apiKeySource, baseUrl, baseUrlSource);
    }

    [McpServerTool, Description("Get authenticated user info.")]
    public static Task<UserInfoResponse> GetUserInfo() => Client.GetUserInfoAsync();

    // ----------------------------
    // Workouts
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of workouts.")]
    public static Task<PaginatedWorkoutsResponse> GetWorkouts(int page = 1, int pageSize = 5) =>
        Client.GetWorkoutsAsync(page, pageSize);

    [McpServerTool, Description("Get the total number of workouts.")]
    public static Task<WorkoutCountResponse> GetWorkoutCount() => Client.GetWorkoutCountAsync();

    [McpServerTool, Description("Get a single workout by workoutId.")]
    public static Task<Workout> GetWorkout(string workoutId) => Client.GetWorkoutAsync(workoutId);

    [McpServerTool, Description("Create a new workout.")]
    public static Task<Workout> CreateWorkout(PostWorkoutsRequestBody request) => Client.CreateWorkoutAsync(request);

    [McpServerTool, Description("Update an existing workout.")]
    public static Task<Workout> UpdateWorkout(string workoutId, PostWorkoutsRequestBody request) =>
        Client.UpdateWorkoutAsync(workoutId, request);

    [McpServerTool, Description("Get paged workout events updated/deleted since a given date.")]
    public static Task<PaginatedWorkoutEventsResponse> GetWorkoutEvents(
        int page = 1,
        int pageSize = 5,
        DateTimeOffset? since = null) =>
        Client.GetWorkoutEventsAsync(page, pageSize, since);

    // ----------------------------
    // Routines
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of routines.")]
    public static Task<PaginatedRoutinesResponse> GetRoutines(int page = 1, int pageSize = 5) =>
        Client.GetRoutinesAsync(page, pageSize);

    [McpServerTool, Description("Get a routine by routineId.")]
    public static Task<Routine> GetRoutine(string routineId) => Client.GetRoutineAsync(routineId);

    [McpServerTool, Description("Create a new routine.")]
    public static Task<Routine> CreateRoutine(PostRoutinesRequestBody request) => Client.CreateRoutineAsync(request);

    [McpServerTool, Description("Update an existing routine.")]
    public static Task<Routine> UpdateRoutine(string routineId, PutRoutinesRequestBody request) =>
        Client.UpdateRoutineAsync(routineId, request);

    // ----------------------------
    // Exercise templates
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of exercise templates.")]
    public static Task<PaginatedExerciseTemplatesResponse> GetExerciseTemplates(int page = 1, int pageSize = 50) =>
        Client.GetExerciseTemplatesAsync(page, pageSize);

    [McpServerTool, Description("Get an exercise template by id.")]
    public static Task<ExerciseTemplate> GetExerciseTemplate(string exerciseTemplateId) =>
        Client.GetExerciseTemplateAsync(exerciseTemplateId);

    [McpServerTool, Description("Create a new custom exercise template.")]
    public static Task<CreateCustomExerciseResponse> CreateCustomExerciseTemplate(CreateCustomExerciseRequestBody request) =>
        Client.CreateCustomExerciseTemplateAsync(request);

    // ----------------------------
    // Routine folders
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of routine folders.")]
    public static Task<PaginatedRoutineFoldersResponse> GetRoutineFolders(int page = 1, int pageSize = 10) =>
        Client.GetRoutineFoldersAsync(page, pageSize);

    [McpServerTool, Description("Get a routine folder by its id.")]
    public static Task<RoutineFolder> GetRoutineFolder(int folderId) => Client.GetRoutineFolderAsync(folderId);

    [McpServerTool, Description("Create a new routine folder.")]
    public static Task<RoutineFolder> CreateRoutineFolder(PostRoutineFolderRequestBody request) =>
        Client.CreateRoutineFolderAsync(request);

    // ----------------------------
    // Exercise history
    // ----------------------------

    [McpServerTool, Description("Get exercise history for a specific exercise template.")]
    public static Task<ExerciseHistoryResponse> GetExerciseHistory(
        string exerciseTemplateId,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null) =>
        Client.GetExerciseHistoryAsync(exerciseTemplateId, startDate, endDate);
}

