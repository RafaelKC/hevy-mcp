using System.Text.Json.Serialization;

namespace HevyClient.DTOs;

public sealed class PaginatedExerciseTemplatesResponse
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("page_count")]
    public int PageCount { get; init; }

    [JsonPropertyName("exercise_templates")]
    public List<ExerciseTemplate> ExerciseTemplates { get; init; } = new();
}

public sealed class ExerciseTemplate
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = "";

    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    [JsonPropertyName("primary_muscle_group")]
    public string PrimaryMuscleGroup { get; init; } = "";

    [JsonPropertyName("secondary_muscle_groups")]
    public List<string> SecondaryMuscleGroups { get; init; } = new();

    [JsonPropertyName("is_custom")]
    public bool IsCustom { get; init; }
}

// ----------------------------
// Requests - Exercise templates
// ----------------------------

public sealed class CreateCustomExerciseRequestBody
{
    [JsonPropertyName("exercise")]
    public CreateCustomExerciseRequest Exercise { get; init; } = new();
}

public sealed class CreateCustomExerciseRequest
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = "";

    [JsonPropertyName("exercise_type")]
    public string ExerciseType { get; init; } = "";

    [JsonPropertyName("equipment_category")]
    public string EquipmentCategory { get; init; } = "";

    [JsonPropertyName("muscle_group")]
    public string MuscleGroup { get; init; } = "";

    [JsonPropertyName("other_muscles")]
    public List<string> OtherMuscles { get; init; } = new();
}

public sealed class CreateCustomExerciseResponse
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
}

