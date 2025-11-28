using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestfromdocspostgresqlLambda.Models
{
    public enum RailcardType
    {
        Young,
        Santander,
        DevonandCornwall,
        TwoTogether,
        Family,
        Senior,
        DisabledPersons,
        Network,
        TwentySixToThirty,
        SixteenToSeventeen,
        Veterans,
        GoldRecordCard
    }

    public enum CardholderType
    {
        Primary,
        Secondary
    }

    // Converter to enforce exact case matching of enum strings in JSON
    public class ExactCaseEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Enum value must be a string");
            var str = reader.GetString() ?? string.Empty;
            if (Enum.TryParse<T>(str, ignoreCase: false, out var value))
                return value;
            throw new JsonException($"Unknown enum value '{str}' for enum type '{typeof(T).Name}'. Case-sensitive match required.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
