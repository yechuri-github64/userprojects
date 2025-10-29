using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using acc-sf-test.Models;

namespace acc-sf-test.Infrastructure
{
 public class SalesforceClient
 {
 private readonly SalesforceSettings _settings;
 private readonly IHttpClientFactory _factory;
 private readonly ILogger<SalesforceClient> _logger;

 public SalesforceClient(SalesforceSettings settings, IHttpClientFactory factory, ILogger<SalesforceClient> logger)
 {
 _settings = settings;
 _factory = factory;
 _logger = logger;
 }

 public HttpClient CreateClient(string instanceUrl, string accessToken, string apiVersion)
 {
 if (string.IsNullOrWhiteSpace(instanceUrl)) throw new ArgumentNullException(nameof(instanceUrl));
 if (!Uri.TryCreate(instanceUrl, UriKind.Absolute, out var baseUri))
 {
 _logger.LogError("Invalid instanceUrl provided to SalesforceClient: {InstanceUrl}", instanceUrl);
 throw new ArgumentException("Invalid instanceUrl");
 }

 var client = _factory.CreateClient();
 client.BaseAddress = new Uri(baseUri, $"/services/data/v{apiVersion}/");
 client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
 client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds > 0 ? _settings.TimeoutSeconds : 120);
 return client;
 }
 }
}
