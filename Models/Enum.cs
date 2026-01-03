using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestLambda.Models
{
    // Example enum used for team category
    [JsonConverter(typeof(ExactCaseJsonStringEnumConverter))]
    public enum TeamCategory
    {
        Professional,
        Amateur
    }

    // Custom converter ensuring exact enum case matching (no renaming/policy)
    public class ExactCaseJsonStringEnumConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum || (Nullable.GetUnderlyingType(typeToConvert)?.IsEnum ?? false);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var underlyingType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
            var converterType = typeof(ExactCaseEnumConverterInternal<>).MakeGenericType(underlyingType);
            return (JsonConverter)Activator.CreateInstance(converterType)!
                ;
        }

        private class ExactCaseEnumConverterInternal<T> : JsonConverter<T> where T : struct, Enum
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var str = reader.GetString();
                    if (str != null && Enum.TryParse<T>(str, ignoreCase: false, out var val))
                    {
                        return val;
                    }
                    throw new JsonException($"Unable to convert '{str}' to enum {typeof(T)} using exact case match.");
                }

                if (reader.TokenType == JsonTokenType.Number)
                {
                    var num = reader.GetInt32();
                    return (T)Enum.ToObject(typeof(T), num);
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
