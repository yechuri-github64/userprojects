namespace demo_test_accounts_salesforce_app.Models
{
 public class SalesforceSettings
 {
 public string ProductionLoginUrl { get; set; }
 public string SandboxLoginUrl { get; set; }
 public bool UseSandbox { get; set; }
 public string ClientId { get; set; }
 public string ClientSecret { get; set; }
 public string Username { get; set; }
 public string Password { get; set; }
 public string SecurityToken { get; set; }
 public string InstanceUrl { get; set; }
 public string ApiVersion { get; set; }
 public int Port { get; set; }
 public string Provider { get; set; }
 }
}
