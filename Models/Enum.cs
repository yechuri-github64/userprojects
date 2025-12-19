using System.Text.Json.Serialization;

namespace CreatetravelcardLambda.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
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

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardholderType
{
    Primary,
    Secondary
}

public static class EnumExtensions
{
    // Ensure exact enum case mapping as string
    public static string ExactEnumCase<T>(this T value) where T : struct, Enum
    {
        var name = Enum.GetName(typeof(T), value);
        return name ?? value.ToString();
    }
}
