using System.Text.Json.Serialization;

namespace CreaterailcardLambda.Models
{
    public class Response
    {
        [JsonPropertyName("railcardId")]
        public string? RailcardId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("error")]
        public ErrorResponse? Error { get; set; }
    }

    public class ErrorResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}
