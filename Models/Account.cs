using System.Text.Json.Serialization;

namespace Models
{
 public class Account
 {
 [JsonPropertyName("id")]
 public string Id { get; set; } = string.Empty;

 [JsonPropertyName("name")]
 public string Name { get; set; } = string.Empty;

 [JsonPropertyName("email")]
 public string Email { get; set; } = string.Empty;

 [JsonPropertyName("address")]
 public string Address { get; set; } = string.Empty;
 }
}