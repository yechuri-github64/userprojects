using System.Text.Json.Serialization;

namespace Test_project_mainLambda
{
    public class ResponseData
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }
    }

    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }
}
