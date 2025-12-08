using System.Text.Json.Serialization;

namespace SalesforceAccountFunctions.Models
{
 public class AccountModel
 {
 [JsonPropertyName("Id")]
 public string? Id { get; set; }

 [JsonPropertyName("Name")]
 public string? Name { get; set; }

 [JsonPropertyName("Phone")]
 public string? Phone { get; set; }

 [JsonPropertyName("BillingCity")]
 public string? BillingCity { get; set; }

 [JsonPropertyName("Industry")]
 public string? Industry { get; set; }

 [JsonPropertyName("Website")]
 public string? Website { get; set; }

 [JsonPropertyName("Description")]
 public string? Description { get; set; }
 }

 public class ErrorResponse
 {
 public string Code { get; set; } = string.Empty;
 public string Message { get; set; } = string.Empty;
 }

 public class BulkCreateRequest
 {
 public List<AccountModel>? Records { get; set; }
 }

 public class CreateSingleRequest
 {
 public AccountModel? Record { get; set; }
 }
}
