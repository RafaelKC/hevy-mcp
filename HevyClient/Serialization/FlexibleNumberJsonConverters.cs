using System.Text.Json;
using System.Text.Json.Serialization;

namespace HevyClient.Serialization;

/// <summary>
/// Parses nullable numbers that sometimes come as JSON numbers and sometimes as strings.
/// </summary>
public sealed class NullableIntFromStringOrNumberConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String =>
                int.TryParse(reader.GetString(), out var v) ? v : null,
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteNumberValue(value.Value);
    }
}

/// <summary>
/// Parses nullable decimals/doubles that sometimes come as JSON numbers and sometimes as strings.
/// </summary>
public sealed class NullableDoubleFromStringOrNumberConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String =>
                double.TryParse(reader.GetString(), out var v) ? v : null,
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteNumberValue(value.Value);
    }
}

