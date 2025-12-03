using System.Text.Json.Serialization;

namespace Models
{
 public class ErrorResponse
 {
 [JsonPropertyName("error")]
 public string Error { get; set; } = string.Empty;

 [JsonPropertyName("details")]
 public string Details { get; set; } = string.Empty;
 }
}