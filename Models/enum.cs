using System.Text.Json.Serialization;
using Npgsql;

namespace CreateaccountsdemoLambda.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActionType
    {
        Get,
        Create,
        Update,
        Delete
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RailcardType
    {
        None,
        Standard,
        Student,
        Senior
    }
}
