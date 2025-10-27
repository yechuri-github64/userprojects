using System.Text.Json.Serialization;

namespace test-acc-sf-app.Models
{
 public class AccountCreateRequest
 {
 [JsonPropertyName("Name")]
 public string Name { get; set; }

 [JsonPropertyName("Phone")]
 public string Phone { get; set; }

 [JsonPropertyName("Website")]
 public string Website { get; set; }

 // Additional Salesforce Account fields can be added as needed
 }
}
