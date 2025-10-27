using System.Text.Json.Serialization;

namespace test-acc-sf-app.Models
{
 public class AccountResponse
 {
 [JsonPropertyName("Id")]
 public string Id { get; set; }

 [JsonPropertyName("Name")]
 public string Name { get; set; }

 [JsonPropertyName("Phone")]
 public string Phone { get; set; }

 [JsonPropertyName("Website")]
 public string Website { get; set; }
 }
}
