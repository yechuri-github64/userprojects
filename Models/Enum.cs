using System.Text.Json.Serialization;
using Npgsql;

namespace CreateaccountsdemoLambda.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OperationType
    {
        Get,
        Create,
        Update,
        Delete
    }

    // Example enum requested by requirements; mapped to Npgsql if needed.
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RailcardType
    {
        None,
        Adult,
        Child,
        Senior
    }

    public static class EnumMapping
    {
        // Called at startup to register enum mapping with Npgsql when used in DB contexts.
        public static void RegisterEnums()
        {
            // Register Npgsql enum mapping. This is safe to call multiple times.
            try
            {
                NpgsqlConnection.GlobalTypeMapper.MapEnum<RailcardType>();
            }
            catch
            {
                // ignore mapping failures (e.g., already mapped)
            }
        }
    }
}
