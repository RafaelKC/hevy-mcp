using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HevyClient.DTOs;
using HevyClient.Serialization;

namespace HevyClient.Client;

public sealed class HevyClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public HevyClient(string apiKey, HevyApiOptions? options = null, HttpClient? httpClient = null)
    {
        options ??= new HevyApiOptions();
        _http = httpClient ?? new HttpClient();

        var baseUrl = options.BaseUrl.TrimEnd('/');
        _http.BaseAddress = new Uri(baseUrl + "/");

        // MCP/clients call every request; keep this deterministic.
        if (_http.DefaultRequestHeaders.Contains("api-key"))
        {
            _http.DefaultRequestHeaders.Remove("api-key");
        }
        _http.DefaultRequestHeaders.Add("api-key", apiKey);

        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonOptions.Converters.Add(new NullableIntFromStringOrNumberConverter());
        _jsonOptions.Converters.Add(new NullableDoubleFromStringOrNumberConverter());
    }

    private async Task<T> ReadJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HevyApiException(
                (int)response.StatusCode,
                response.ReasonPhrase ?? "Request failed",
                content);
        }

        var parsed = JsonSerializer.Deserialize<T>(content, _jsonOptions);
        if (parsed is null)
            throw new HevyApiException((int)response.StatusCode, "Failed to parse JSON response", content);

        return parsed;
    }

    private string BuildUrl(string path, Dictionary<string, string?> query)
    {
        var sb = new StringBuilder();
        sb.Append(path);

        var first = true;
        foreach (var kvp in query)
        {
            if (string.IsNullOrWhiteSpace(kvp.Value)) continue;
            sb.Append(first ? '?' : '&');
            first = false;
            sb.Append(Uri.EscapeDataString(kvp.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kvp.Value));
        }

        return sb.ToString();
    }

    private async Task<T> SendAsync<T>(
        HttpMethod method,
        string url,
        object? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: _jsonOptions);
        }

        using var response = await _http.SendAsync(request, cancellationToken);
        return await ReadJsonAsync<T>(response, cancellationToken);
    }

    // ----------------------------
    // Workouts
    // ----------------------------
    public Task<PaginatedWorkoutsResponse> GetWorkoutsAsync(
        int page = 1,
        int pageSize = 5,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("/v1/workouts", new()
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        });
        return SendAsync<PaginatedWorkoutsResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<WorkoutCountResponse> GetWorkoutCountAsync(CancellationToken cancellationToken = default)
    {
        var url = "/v1/workouts/count";
        return SendAsync<WorkoutCountResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<PaginatedWorkoutEventsResponse> GetWorkoutEventsAsync(
        int page = 1,
        int pageSize = 5,
        DateTimeOffset? since = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("/v1/workouts/events", new()
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["since"] = since?.ToString("o")
        });

        return SendAsync<PaginatedWorkoutEventsResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<Workout> GetWorkoutAsync(
        string workoutId,
        CancellationToken cancellationToken = default)
    {
        var url = $"/v1/workouts/{Uri.EscapeDataString(workoutId)}";
        return SendAsync<Workout>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<Workout> CreateWorkoutAsync(
        PostWorkoutsRequestBody request,
        CancellationToken cancellationToken = default)
    {
        var url = "/v1/workouts";
        return SendAsync<Workout>(HttpMethod.Post, url, request, cancellationToken);
    }

    public Task<Workout> UpdateWorkoutAsync(
        string workoutId,
        PostWorkoutsRequestBody request,
        CancellationToken cancellationToken = default)
    {
        var url = $"/v1/workouts/{Uri.EscapeDataString(workoutId)}";
        return SendAsync<Workout>(HttpMethod.Put, url, request, cancellationToken);
    }

    // ----------------------------
    // User
    // ----------------------------
    public Task<UserInfoResponse> GetUserInfoAsync(CancellationToken cancellationToken = default)
    {
        var url = "/v1/user/info";
        return SendAsync<UserInfoResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    // ----------------------------
    // Routines
    // ----------------------------
    public Task<PaginatedRoutinesResponse> GetRoutinesAsync(
        int page = 1,
        int pageSize = 5,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("/v1/routines", new()
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        });
        return SendAsync<PaginatedRoutinesResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<Routine> GetRoutineAsync(string routineId, CancellationToken cancellationToken = default)
    {
        var url = $"/v1/routines/{Uri.EscapeDataString(routineId)}";
        return SendAsync<Routine>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<Routine> CreateRoutineAsync(
        PostRoutinesRequestBody request,
        CancellationToken cancellationToken = default)
    {
        var url = "/v1/routines";
        return SendAsync<Routine>(HttpMethod.Post, url, request, cancellationToken);
    }

    public Task<Routine> UpdateRoutineAsync(
        string routineId,
        PutRoutinesRequestBody request,
        CancellationToken cancellationToken = default)
    {
        var url = $"/v1/routines/{Uri.EscapeDataString(routineId)}";
        return SendAsync<Routine>(HttpMethod.Put, url, request, cancellationToken);
    }

    // ----------------------------
    // Exercise templates
    // ----------------------------
    public Task<PaginatedExerciseTemplatesResponse> GetExerciseTemplatesAsync(
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("/v1/exercise_templates", new()
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        });
        return SendAsync<PaginatedExerciseTemplatesResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<ExerciseTemplate> GetExerciseTemplateAsync(
        string exerciseTemplateId,
        CancellationToken cancellationToken = default)
    {
        var url = $"/v1/exercise_templates/{Uri.EscapeDataString(exerciseTemplateId)}";
        return SendAsync<ExerciseTemplate>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<CreateCustomExerciseResponse> CreateCustomExerciseTemplateAsync(
        CreateCustomExerciseRequestBody request,
        CancellationToken cancellationToken = default)
    {
        var url = "/v1/exercise_templates";
        return SendAsync<CreateCustomExerciseResponse>(HttpMethod.Post, url, request, cancellationToken);
    }

    // ----------------------------
    // Routine folders
    // ----------------------------
    public Task<PaginatedRoutineFoldersResponse> GetRoutineFoldersAsync(
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl("/v1/routine_folders", new()
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        });
        return SendAsync<PaginatedRoutineFoldersResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<RoutineFolder> GetRoutineFolderAsync(
        int folderId,
        CancellationToken cancellationToken = default)
    {
        var url = $"/v1/routine_folders/{folderId}";
        return SendAsync<RoutineFolder>(HttpMethod.Get, url, body: null, cancellationToken);
    }

    public Task<RoutineFolder> CreateRoutineFolderAsync(
        PostRoutineFolderRequestBody request,
        CancellationToken cancellationToken = default)
    {
        var url = "/v1/routine_folders";
        return SendAsync<RoutineFolder>(HttpMethod.Post, url, request, cancellationToken);
    }

    // ----------------------------
    // Exercise history
    // ----------------------------
    public Task<ExerciseHistoryResponse> GetExerciseHistoryAsync(
        string exerciseTemplateId,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl($"/v1/exercise_history/{Uri.EscapeDataString(exerciseTemplateId)}", new()
        {
            ["start_date"] = startDate?.ToString("o"),
            ["end_date"] = endDate?.ToString("o")
        });

        return SendAsync<ExerciseHistoryResponse>(HttpMethod.Get, url, body: null, cancellationToken);
    }
}

public sealed class HevyApiException : Exception
{
    public int StatusCode { get; }
    public string ResponseBody { get; }

    public HevyApiException(int statusCode, string message, string responseBody) : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}

