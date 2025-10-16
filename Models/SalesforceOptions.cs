namespace accounts_sf_sa.Models
{
    public class SalesforceOptions
    {
        public string Provider { get; set; } = "Salesforce";
        public string ApiVersion { get; set; } = "59.0";
        public bool UseSandbox { get; set; } = false;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SecurityToken { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string AuthUrl { get; set; } = string.Empty;
        public string InstanceUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 100;
    }
}
