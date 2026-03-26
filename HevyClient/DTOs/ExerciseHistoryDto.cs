using System.Text.Json.Serialization;
using HevyClient.Serialization;

namespace HevyClient.DTOs;

public sealed class ExerciseHistoryResponse
{
    [JsonPropertyName("exercise_history")]
    public List<ExerciseHistoryEntry> ExerciseHistory { get; init; } = new();
}

public sealed class ExerciseHistoryEntry
{
    [JsonPropertyName("workout_id")]
    public string WorkoutId { get; init; } = "";

    [JsonPropertyName("workout_title")]
    public string WorkoutTitle { get; init; } = "";

    [JsonPropertyName("workout_start_time")]
    public string WorkoutStartTime { get; init; } = "";

    [JsonPropertyName("workout_end_time")]
    public string WorkoutEndTime { get; init; } = "";

    [JsonPropertyName("exercise_template_id")]
    public string ExerciseTemplateId { get; init; } = "";

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

    [JsonPropertyName("set_type")]
    public string SetType { get; init; } = "";
}

