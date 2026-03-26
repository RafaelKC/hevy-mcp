using System.ComponentModel;
using HevyClient.Client;
using HevyClient.DTOs;
using HevyMcp.Auth;
using ModelContextProtocol.Server;

namespace HevyMcp;

[McpServerToolType]
public sealed class HevyTrainerTools
{
    public sealed record HevyConfigStatus(
        string EnvironmentName,
        string? BaseUrl,
        bool HasBearer,
        bool HasHevyApiKeyInRequest);

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HevyTrainerTools(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private (string HevyApiKey, string? BaseUrl) GetRequestHevyConfig()
    {
        var ctx = _httpContextAccessor.HttpContext
                  ?? throw new InvalidOperationException("No HttpContext available. This tool must be called over HTTP transport.");

        var apiKey = ctx.Items[HevyMcpTokenMiddleware.HttpContextItemApiKey] as string;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Missing Hevy API key in request context. Provide a valid Authorization: Bearer token.");

        var baseUrl = ctx.Items[HevyMcpTokenMiddleware.HttpContextItemBaseUrl] as string;
        return (apiKey, baseUrl);
    }

    private HevyClient.Client.HevyClient CreateClientForRequest()
    {
        var (apiKey, baseUrl) = GetRequestHevyConfig();
        var options = !string.IsNullOrWhiteSpace(baseUrl) ? new HevyApiOptions { BaseUrl = baseUrl } : null;
        return new HevyClient.Client.HevyClient(apiKey, options);
    }

    [McpServerTool, Description("Get authenticated user info.")]
    public Task<UserInfoResponse> GetUserInfo() => CreateClientForRequest().GetUserInfoAsync();

    [McpServerTool, Description("Diagnose request auth context. Does not reveal secrets.")]
    public HevyConfigStatus GetHevyConfigStatus()
    {
        var ctx = _httpContextAccessor.HttpContext;
        var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                      ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                      ?? "Production";

        var hasBearer = ctx?.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true;
        var hasKey = ctx?.Items.ContainsKey(HevyMcpTokenMiddleware.HttpContextItemApiKey) == true;
        var baseUrl = ctx?.Items[HevyMcpTokenMiddleware.HttpContextItemBaseUrl] as string;

        return new HevyConfigStatus(
            EnvironmentName: envName,
            BaseUrl: baseUrl,
            HasBearer: hasBearer,
            HasHevyApiKeyInRequest: hasKey);
    }

    // ----------------------------
    // Workouts
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of workouts.")]
    public Task<PaginatedWorkoutsResponse> GetWorkouts(int page = 1, int pageSize = 5) =>
        CreateClientForRequest().GetWorkoutsAsync(page, pageSize);

    [McpServerTool, Description("Get the total number of workouts.")]
    public Task<WorkoutCountResponse> GetWorkoutCount() => CreateClientForRequest().GetWorkoutCountAsync();

    [McpServerTool, Description("Get a single workout by workoutId.")]
    public Task<Workout> GetWorkout(string workoutId) => CreateClientForRequest().GetWorkoutAsync(workoutId);

    [McpServerTool, Description("Create a new workout.")]
    public Task<Workout> CreateWorkout(PostWorkoutsRequestBody request) => CreateClientForRequest().CreateWorkoutAsync(request);

    [McpServerTool, Description("Update an existing workout.")]
    public Task<Workout> UpdateWorkout(string workoutId, PostWorkoutsRequestBody request) =>
        CreateClientForRequest().UpdateWorkoutAsync(workoutId, request);

    [McpServerTool, Description("Get paged workout events updated/deleted since a given date.")]
    public Task<PaginatedWorkoutEventsResponse> GetWorkoutEvents(
        int page = 1,
        int pageSize = 5,
        DateTimeOffset? since = null) =>
        CreateClientForRequest().GetWorkoutEventsAsync(page, pageSize, since);

    // ----------------------------
    // Routines
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of routines.")]
    public Task<PaginatedRoutinesResponse> GetRoutines(int page = 1, int pageSize = 5) =>
        CreateClientForRequest().GetRoutinesAsync(page, pageSize);

    [McpServerTool, Description("Get a routine by routineId.")]
    public Task<Routine> GetRoutine(string routineId) => CreateClientForRequest().GetRoutineAsync(routineId);

    [McpServerTool, Description("Create a new routine.")]
    public Task<Routine> CreateRoutine(PostRoutinesRequestBody request) => CreateClientForRequest().CreateRoutineAsync(request);

    [McpServerTool, Description("Update an existing routine.")]
    public Task<Routine> UpdateRoutine(string routineId, PutRoutinesRequestBody request) =>
        CreateClientForRequest().UpdateRoutineAsync(routineId, request);

    // ----------------------------
    // Exercise templates
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of exercise templates.")]
    public Task<PaginatedExerciseTemplatesResponse> GetExerciseTemplates(int page = 1, int pageSize = 50) =>
        CreateClientForRequest().GetExerciseTemplatesAsync(page, pageSize);

    [McpServerTool, Description("Get an exercise template by id.")]
    public Task<ExerciseTemplate> GetExerciseTemplate(string exerciseTemplateId) =>
        CreateClientForRequest().GetExerciseTemplateAsync(exerciseTemplateId);

    [McpServerTool, Description("Create a new custom exercise template.")]
    public Task<CreateCustomExerciseResponse> CreateCustomExerciseTemplate(CreateCustomExerciseRequestBody request) =>
        CreateClientForRequest().CreateCustomExerciseTemplateAsync(request);

    // ----------------------------
    // Routine folders
    // ----------------------------

    [McpServerTool, Description("Get a paginated list of routine folders.")]
    public Task<PaginatedRoutineFoldersResponse> GetRoutineFolders(int page = 1, int pageSize = 10) =>
        CreateClientForRequest().GetRoutineFoldersAsync(page, pageSize);

    [McpServerTool, Description("Get a routine folder by its id.")]
    public Task<RoutineFolder> GetRoutineFolder(int folderId) => CreateClientForRequest().GetRoutineFolderAsync(folderId);

    [McpServerTool, Description("Create a new routine folder.")]
    public Task<RoutineFolder> CreateRoutineFolder(PostRoutineFolderRequestBody request) =>
        CreateClientForRequest().CreateRoutineFolderAsync(request);

    // ----------------------------
    // Exercise history
    // ----------------------------

    [McpServerTool, Description("Get exercise history for a specific exercise template.")]
    public Task<ExerciseHistoryResponse> GetExerciseHistory(
        string exerciseTemplateId,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null) =>
        CreateClientForRequest().GetExerciseHistoryAsync(exerciseTemplateId, startDate, endDate);
}

