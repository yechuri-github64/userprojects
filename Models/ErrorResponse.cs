using System.Text.Json.Serialization;

namespace Models
{
 public class ErrorResponse
 {
 [JsonPropertyName("code")]
 public string Code { get; set; } = "error";

 [JsonPropertyName("message")]
 public string Message { get; set; } = string.Empty;

 [JsonPropertyName("details")]
 public object? Details { get; set; }
 }
}