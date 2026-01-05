using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

#nullable enable

namespace DemotestprojLambda.Models
{
    // Custom factory to ensure exact enum case is used when serializing/deserializing
    public class ExactEnumStringConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var enumType = typeToConvert;
            var converterType = typeof(ExactEnumStringConverter<>).MakeGenericType(enumType);
            return (JsonConverter?)Activator.CreateInstance(converterType)!;
        }
    }

    public class ExactEnumStringConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (s is null)
                throw new JsonException($"Cannot convert null to enum {typeof(T)}");

            if (Enum.TryParse<T>(s, ignoreCase: false, out var value))
                return value;

            throw new JsonException($"Unknown value '{s}' for enum {typeof(T)}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    [JsonConverter(typeof(ExactEnumStringConverterFactory))]
    public enum TravelcardType
    {
        Young,
        Barcklays,
        DevonandCornwall,
        TwoTogether,
        Family,
        Senior,
        DisabledPersons,
        Network,
        TwentySixToThirty,
        SixteenToSeventeen,
        Veterans
    }

    [JsonConverter(typeof(ExactEnumStringConverterFactory))]
    public enum CardholderType
    {
        Primary,
        Secondary
    }
}
