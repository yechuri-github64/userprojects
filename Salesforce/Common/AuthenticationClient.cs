namespace Salesforce.Common
{
    public class AuthenticationClient
    {
        public string ApiVersion { get; set; } = "v58.0";
        public string InstanceUrl { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;

        public Task UsernamePasswordAsync(string clientId, string clientSecret, string username, string password, string loginUrl)
        {
            this.InstanceUrl = "https://mock-instance.salesforce.com";
            this.AccessToken = "mock-access-token";
            return Task.CompletedTask;
        }
    }
}
