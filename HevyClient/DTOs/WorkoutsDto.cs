using System.Text.Json.Serialization;
using HevyClient.Serialization;

namespace HevyClient.DTOs;

public sealed class PaginatedWorkoutsResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("workouts")]
    public List<Workout> Workouts { get; init; } = new();
}

public sealed class Workout
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("routine_id")]
    public string? RoutineId { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("start_time")]
    public string StartTime { get; init; } = "";

    [JsonPropertyName("end_time")]
    public string EndTime { get; init; } = "";

    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; init; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; init; }

    [JsonPropertyName("exercises")]
    public List<WorkoutExercise> Exercises { get; init; } = new();
}

public sealed class WorkoutExercise
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("exercise_template_id")]
    public string ExerciseTemplateId { get; init; } = "";

    [JsonPropertyName("supersets_id")]
    public int? SupersetsId { get; init; }

    [JsonPropertyName("sets")]
    public List<WorkoutSet> Sets { get; init; } = new();
}

public sealed class WorkoutSet
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    [JsonPropertyName("weight_kg")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? WeightKg { get; init; }

    [JsonPropertyName("reps")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? Reps { get; init; }

    [JsonPropertyName("distance_meters")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? DistanceMeters { get; init; }

    [JsonPropertyName("duration_seconds")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? DurationSeconds { get; init; }

    [JsonPropertyName("rpe")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? Rpe { get; init; }

    [JsonPropertyName("custom_metric")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? CustomMetric { get; init; }
}

public sealed class WorkoutCountResponse
{
    [JsonPropertyName("workout_count")]
    public int WorkoutCount { get; init; }
}

public sealed class PaginatedWorkoutEventsResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("events")]
    public List<WorkoutEvent> Events { get; init; } = new();
}

public sealed class WorkoutEvent
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    // Present for "updated" events.
    [JsonPropertyName("workout")]
    public Workout? Workout { get; init; }

    // Present for "deleted" events.
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("deleted_at")]
    public string? DeletedAt { get; init; }
}

// ----------------------------
// Requests - Workouts
// ----------------------------

public sealed class PostWorkoutsRequestBody
{
    [JsonPropertyName("workout")]
    public PostWorkoutsRequestWorkout Workout { get; init; } = new();
}

public sealed class PostWorkoutsRequestWorkout
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("start_time")]
    public string StartTime { get; init; } = "";

    [JsonPropertyName("end_time")]
    public string EndTime { get; init; } = "";

    [JsonPropertyName("is_private")]
    public bool IsPrivate { get; init; }

    [JsonPropertyName("exercises")]
    public List<PostWorkoutsRequestExercise> Exercises { get; init; } = new();
}

public sealed class PostWorkoutsRequestExercise
{
    [JsonPropertyName("exercise_template_id")]
    public string ExerciseTemplateId { get; init; } = "";

    [JsonPropertyName("superset_id")]
    public int? SupersetId { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("sets")]
    public List<PostWorkoutsRequestSet> Sets { get; init; } = new();
}

public sealed class PostWorkoutsRequestSet
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    [JsonPropertyName("weight_kg")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? WeightKg { get; init; }

    [JsonPropertyName("reps")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? Reps { get; init; }

    [JsonPropertyName("distance_meters")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? DistanceMeters { get; init; }

    [JsonPropertyName("duration_seconds")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? DurationSeconds { get; init; }

    [JsonPropertyName("custom_metric")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? CustomMetric { get; init; }

    [JsonPropertyName("rpe")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? Rpe { get; init; }
}

