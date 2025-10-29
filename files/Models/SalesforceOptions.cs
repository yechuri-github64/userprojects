namespace acc_sf_test.Models
{
 public class RetryPolicyOptions
 {
 public int MaxRetries { get; set; } = 3;
 public int DelaySeconds { get; set; } = 3;
 }

 public class SalesforceOptions
 {
 public string Provider { get; set; } = "Salesforce";
 public string ApiVersion { get; set; } = "59.0";
 public bool UseSandbox { get; set; } = false;
 public string ClientId { get; set; } = "";
 public string ClientSecret { get; set; } = "";
 public string Username { get; set; } = "";
 public string Password { get; set; } = "";
 public string SecurityToken { get; set; } = "";
 public string TokenUrl { get; set; } = "";
 public string AuthUrl { get; set; } = "";
 public string InstanceUrl { get; set; } = "";
 public int TimeoutSeconds { get; set; } = 120;
 public bool EnableLogging { get; set; } = true;
 public RetryPolicyOptions RetryPolicy { get; set; } = new RetryPolicyOptions();

 // runtime state
 public string? AccessToken { get; set; }
 }
}