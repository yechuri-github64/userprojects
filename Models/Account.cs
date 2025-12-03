using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Models
{
 public class Account
 {
 [JsonPropertyName("id")]
 public string Id { get; set; } = string.Empty;

 [JsonPropertyName("name")]
 [Required]
 public string Name { get; set; } = string.Empty;

 [JsonPropertyName("email")]
 [EmailAddress]
 public string Email { get; set; } = string.Empty;

 [JsonPropertyName("address")]
 public string Address { get; set; } = string.Empty;
 }
}