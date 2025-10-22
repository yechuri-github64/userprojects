using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using acc_sf_test.Models;

namespace acc_sf_test.Services
{
    // Minimal Salesforce REST client implementation for creating/updating/deleting Account sobjects.
    public class SalesforceClient
    {
        private readonly HttpClient _http;
        private readonly string _apiVersion;
        private readonly ILogger<SalesforceClient> _logger;

        public SalesforceClient(HttpClient http, IConfiguration config, ILogger<SalesforceClient> logger)
        {
            _http = http;
            _logger = logger;
            _apiVersion = config.GetValue<string>("Salesforce:ApiVersion") ?? "v57.0";

            // Assume authentication handled externally or via headers configured in Program.cs.
        }

        public async Task<string> CreateAccountAsync(AccountRequest req)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));

            var payload = new
            {
                Name = req.Name,
                Industry = req.Industry,
                Phone = req.Phone,
                Website = req.Website
            };

            var response = await _http.PostAsJsonAsync($"/services/data/{_apiVersion}/sobjects/Account/", payload);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Salesforce create failed: {Status} {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Salesforce create failed: {response.ReasonPhrase}");
            }

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (doc.RootElement.TryGetProperty("id", out var idElem))
            {
                return idElem.GetString();
            }

            return null;
        }

        public async Task UpdateAccountAsync(string salesforceId, AccountRequest req)
        {
            if (string.IsNullOrWhiteSpace(salesforceId)) throw new ArgumentNullException(nameof(salesforceId));

            var payload = new
            {
                Name = req.Name,
                Industry = req.Industry,
                Phone = req.Phone,
                Website = req.Website
            };

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/services/data/{_apiVersion}/sobjects/Account/{salesforceId}")
            {
                Content = JsonContent.Create(payload)
            };

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Salesforce update failed: {Status} {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Salesforce update failed: {response.ReasonPhrase}");
            }
        }

        public async Task DeleteAccountAsync(string salesforceId)
        {
            if (string.IsNullOrWhiteSpace(salesforceId)) throw new ArgumentNullException(nameof(salesforceId));

            var response = await _http.DeleteAsync($"/services/data/{_apiVersion}/sobjects/Account/{salesforceId}");
            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Salesforce delete failed: {Status} {Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Salesforce delete failed: {response.ReasonPhrase}");
            }
        }
    }
}
