using System.Text.Json.Serialization;

namespace Test_project_mainLambda
{
    // Optional request data structure (GET uses query parameters instead of a JSON body)
    public class RequestData
    {
        [JsonPropertyName("min")]
        public int? Min { get; set; }

        [JsonPropertyName("max")]
        public int? Max { get; set; }
    }
}
