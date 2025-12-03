namespace Salesforce.Force
{
    public class ForceClient
    {
        public ForceClient(string instanceUrl, string accessToken, string apiVersion = "v60.0")
        {
        }

        public async Task<dynamic> CreateAsync<T>(string sObjectTypeName, object fields) where T : class
        {
            return new { id = Guid.NewGuid().ToString() };
        }

        public async Task<dynamic> QueryAsync<T>(string query) where T : class
        {
            return new { records = new List<dynamic>() };
        }

        public async Task<T> RetrieveAsync<T>(string sObjectTypeName, string recordId, IList<string> fields) where T : class
        {
            return null!;
        }

        public async Task UpdateAsync(string sObjectTypeName, string recordId, object fields)
        {
        }

        public async Task DeleteAsync(string sObjectTypeName, string recordId)
        {
        }
    }
}
