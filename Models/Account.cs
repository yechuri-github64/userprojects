using System.Text.Json.Serialization;

namespace AccountManagerFunctionApp.Models
{
 public class Account
 {
 [JsonPropertyName("id")]
 public string? Id { get; set; }

 [JsonPropertyName("name")]
 public string? Name { get; set; }

 [JsonPropertyName("email")]
 public string? Email { get; set; }

 [JsonPropertyName("address")]
 public string? Address { get; set; }
 }
}
