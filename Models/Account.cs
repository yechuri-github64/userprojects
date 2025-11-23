using System.Text.Json.Serialization;

namespace demo_test_accounts_salesforce_app.Models
{
 // Represents the standard Salesforce Account object JSON structure used for create/update/retrieve
 public class Account
 {
 [JsonPropertyName("Id")]
 public string Id { get; set; }

 [JsonPropertyName("Name")]
 public string Name { get; set; }

 [JsonPropertyName("Phone")]
 public string Phone { get; set; }

 [JsonPropertyName("Website")]
 public string Website { get; set; }

 [JsonPropertyName("Industry")]
 public string Industry { get; set; }

 [JsonPropertyName("Type")]
 public string Type { get; set; }

 [JsonPropertyName("BillingStreet")]
 public string BillingStreet { get; set; }

 [JsonPropertyName("BillingCity")]
 public string BillingCity { get; set; }

 [JsonPropertyName("BillingState")]
 public string BillingState { get; set; }

 [JsonPropertyName("BillingPostalCode")]
 public string BillingPostalCode { get; set; }

 [JsonPropertyName("BillingCountry")]
 public string BillingCountry { get; set; }

 [JsonPropertyName("CreatedDate")]
 public string CreatedDate { get; set; }

 [JsonPropertyName("LastModifiedDate")]
 public string LastModifiedDate { get; set; }
 }
}
