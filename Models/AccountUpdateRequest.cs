using System.Text.Json.Serialization;

namespace test-acc-sf-app.Models
{
 public class AccountUpdateRequest
 {
 [JsonPropertyName("Name")]
 public string Name { get; set; }

 [JsonPropertyName("Phone")]
 public string Phone { get; set; }

 [JsonPropertyName("Website")]
 public string Website { get; set; }
 }
}
