using acc_sf_test.Models;

namespace acc_sf_test.Services
{
 public class ValidationResult
 {
 public bool IsValid { get; set; } = true;
 public List<string> Errors { get; } = new List<string>();
 }

 public class SalesforceOptionsValidator : IValidateOptions<SalesforceOptions>
 {
 public ValidateOptionsResult Validate(string name, SalesforceOptions options)
 {
 var result = Validate(options);
 if (result.IsValid) return ValidateOptionsResult.Success;
 else return ValidateOptionsResult.Fail(result.Errors.ToArray());
 }

 public ValidationResult Validate(SalesforceOptions options)
 {
 var res = new ValidationResult();
 if (options == null)
 {
 res.IsValid = false;
 res.Errors.Add("Salesforce options cannot be null");
 return res;
 }

 // Provider and ApiVersion fallback to defaults if empty
 if (string.IsNullOrWhiteSpace(options.Provider)) options.Provider = "Salesforce";
 if (string.IsNullOrWhiteSpace(options.ApiVersion)) options.ApiVersion = "59.0";

 // Credentials are required
 if (string.IsNullOrWhiteSpace(options.ClientId)) res.Errors.Add("ClientId is required");
 if (string.IsNullOrWhiteSpace(options.ClientSecret)) res.Errors.Add("ClientSecret is required");
 if (string.IsNullOrWhiteSpace(options.Username)) res.Errors.Add("Username is required");
 if (string.IsNullOrWhiteSpace(options.Password)) res.Errors.Add("Password is required");

 // Provide defaults for urls to avoid null
 if (string.IsNullOrWhiteSpace(options.TokenUrl)) options.TokenUrl = options.UseSandbox ? "https://test.salesforce.com/services/oauth2/token" : "https://login.salesforce.com/services/oauth2/token";
 if (string.IsNullOrWhiteSpace(options.AuthUrl)) options.AuthUrl = options.UseSandbox ? "https://test.salesforce.com" : "https://login.salesforce.com";
 if (string.IsNullOrWhiteSpace(options.InstanceUrl)) options.InstanceUrl = options.UseSandbox ? "https://your-sandbox.my.salesforce.com" : "https://your-instance.my.salesforce.com";

 // Validate URIs
 if (!Uri.TryCreate(options.TokenUrl, UriKind.Absolute, out _)) res.Errors.Add("TokenUrl is not a valid absolute URI");
 if (!Uri.TryCreate(options.AuthUrl, UriKind.Absolute, out _)) res.Errors.Add("AuthUrl is not a valid absolute URI");
 if (!Uri.TryCreate(options.InstanceUrl, UriKind.Absolute, out _)) res.Errors.Add("InstanceUrl is not a valid absolute URI");

 if (res.Errors.Count > 0) res.IsValid = false;
 return res;
 }
 }
}