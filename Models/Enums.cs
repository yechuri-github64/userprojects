using System.Text.Json.Serialization;

namespace CreaterailcardLambda.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CardholderType
    {
        Primary,
        Secondary
    }
}
