using System;

namespace acc-sf-test.Models
{
 public class SalesforceSettings
 {
 public string? Provider { get; set; }
 public string? ApiVersion { get; set; }
 public bool UseSandbox { get; set; }
 public string? ClientId { get; set; }
 public string? ClientSecret { get; set; }
 public string? Username { get; set; }
 public string? Password { get; set; }
 public string? SecurityToken { get; set; }
 public string? TokenUrl { get; set; }
 public string? AuthUrl { get; set; }
 public string? InstanceUrl { get; set; }
 public int TimeoutSeconds { get; set; } = 120;
 public bool EnableLogging { get; set; } = true;
 public RetryPolicy? RetryPolicy { get; set; }
 }

 public class RetryPolicy
 {
 public int MaxRetries { get; set; } = 3;
 public int DelaySeconds { get; set; } = 3;
 }
}
