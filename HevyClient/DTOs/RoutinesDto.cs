using System.Text.Json.Serialization;
using HevyClient.Serialization;

namespace HevyClient.DTOs;

public sealed class PaginatedRoutinesResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("routines")]
    public List<Routine> Routines { get; init; } = new();
}

public sealed class Routine
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("folder_id")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? FolderId { get; init; }

    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; init; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; init; }

    [JsonPropertyName("exercises")]
    public List<RoutineExercise> Exercises { get; init; } = new();
}

public sealed class RoutineExercise
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("rest_seconds")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? RestSeconds { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("exercise_template_id")]
    public string ExerciseTemplateId { get; init; } = "";

    [JsonPropertyName("supersets_id")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? SupersetsId { get; init; }

    [JsonPropertyName("sets")]
    public List<RoutineSet> Sets { get; init; } = new();
}

public sealed class RoutineSet
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

    [JsonPropertyName("rep_range")]
    public RepRange? RepRange { get; init; }

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

public sealed class RepRange
{
    [JsonPropertyName("start")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? Start { get; init; }

    [JsonPropertyName("end")]
    [JsonConverter(typeof(NullableDoubleFromStringOrNumberConverter))]
    public double? End { get; init; }
}

// ----------------------------
// Requests - Routines (POST/PUT)
// ----------------------------

public sealed class PostRoutinesRequestBody
{
    [JsonPropertyName("routine")]
    public PostRoutinesRequestRoutine Routine { get; init; } = new();
}

public sealed class PostRoutinesRequestRoutine
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("folder_id")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? FolderId { get; init; }

    [JsonPropertyName("notes")]
    public string Notes { get; init; } = "";

    [JsonPropertyName("exercises")]
    public List<PostRoutinesRequestExercise> Exercises { get; init; } = new();
}

public sealed class PostRoutinesRequestExercise
{
    [JsonPropertyName("exercise_template_id")]
    public string ExerciseTemplateId { get; init; } = "";

    [JsonPropertyName("superset_id")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? SupersetId { get; init; }

    [JsonPropertyName("rest_seconds")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? RestSeconds { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("sets")]
    public List<PostRoutinesRequestSet> Sets { get; init; } = new();
}

public sealed class PostRoutinesRequestSet
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

    [JsonPropertyName("rep_range")]
    public RepRange? RepRange { get; init; }
}

public sealed class PutRoutinesRequestBody
{
    [JsonPropertyName("routine")]
    public PutRoutinesRequestRoutine Routine { get; init; } = new();
}

public sealed class PutRoutinesRequestRoutine
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("exercises")]
    public List<PutRoutinesRequestExercise> Exercises { get; init; } = new();
}

public sealed class PutRoutinesRequestExercise
{
    [JsonPropertyName("exercise_template_id")]
    public string ExerciseTemplateId { get; init; } = "";

    [JsonPropertyName("superset_id")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? SupersetId { get; init; }

    [JsonPropertyName("rest_seconds")]
    [JsonConverter(typeof(NullableIntFromStringOrNumberConverter))]
    public int? RestSeconds { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }

    [JsonPropertyName("sets")]
    public List<PutRoutinesRequestSet> Sets { get; init; } = new();
}

public sealed class PutRoutinesRequestSet
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

    [JsonPropertyName("rep_range")]
    public RepRange? RepRange { get; init; }
}

