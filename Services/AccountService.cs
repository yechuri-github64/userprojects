using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using accounts_management_c_sharp.Data;
using accounts_management_c_sharp.Models;

namespace accounts_management_c_sharp.Services
{
 public class AccountService : IAccountService
 {
 private readonly ApplicationDbContext _dbContext;
 private readonly ISalesforceClient _salesforceClient;
 private readonly IConfiguration _configuration;
 private readonly ILogger<AccountService> _logger;

 public AccountService(ApplicationDbContext dbContext, ISalesforceClient salesforceClient, IConfiguration configuration, ILogger<AccountService> logger)
 {
 _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
 _salesforceClient = salesforceClient ?? throw new ArgumentNullException(nameof(salesforceClient));
 _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
 _logger = logger ?? throw new ArgumentNullException(nameof(logger));
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 var provider = _configuration.GetValue<string>("Backend:Provider")?.ToLowerInvariant();
 if (provider == "salesforce")
 {
 // Example: query Salesforce Accounts (limited simple example)
 var q = "SELECT Id, Name, Type FROM Account LIMIT 200";
 var resp = await _salesforceClient.QueryAsync(q);
 if (!resp.IsSuccessStatusCode) throw new Exception("Salesforce query failed: " + resp.ReasonPhrase);
 var body = await resp.Content.ReadAsStringAsync();
 // Return raw JSON wrapped in Account objects may be lossy; here we return empty list and allow direct Salesforce endpoints
 _logger.LogInformation("Returning empty list for SF query; use Salesforce endpoints to fetch raw data.");
 return Array.Empty<Account>();
 }

 using var conn = _dbContext.CreateConnection();
 await conn.OpenAsync();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT id, name, email, address FROM accounts";
 var list = new List<Account>();
 using var reader = await cmd.ExecuteReaderAsync();
 while (await reader.ReadAsync())
 {
 list.Add(new Account
 {
 Id = reader.GetInt32(0),
 Name = reader.IsDBNull(1) ? null : reader.GetString(1),
 Email = reader.IsDBNull(2) ? null : reader.GetString(2),
 Address = reader.IsDBNull(3) ? null : reader.GetString(3)
 });
 }
 return list;
 }

 public async Task<Account> GetByIdAsync(int id)
 {
 var provider = _configuration.GetValue<string>("Backend:Provider")?.ToLowerInvariant();
 if (provider == "salesforce")
 {
 // Salesforce expects external id or SF id; we assume id is SF Id numeric not likely; provide separate endpoints for SF
 throw new NotSupportedException("Fetching by numeric id is not supported for Salesforce backend via this method. Use Salesforce endpoints.");
 }

 using var conn = _dbContext.CreateConnection();
 await conn.OpenAsync();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT id, name, email, address FROM accounts WHERE id = @id";
 var p = cmd.CreateParameter();
 p.ParameterName = "@id";
 p.Value = id;
 cmd.Parameters.Add(p);
 using var reader = await cmd.ExecuteReaderAsync();
 if (!await reader.ReadAsync()) return null;
 return new Account
 {
 Id = reader.GetInt32(0),
 Name = reader.IsDBNull(1) ? null : reader.GetString(1),
 Email = reader.IsDBNull(2) ? null : reader.GetString(2),
 Address = reader.IsDBNull(3) ? null : reader.GetString(3)
 };
 }

 public async Task<IEnumerable<Account>> CreateManyAsync(IEnumerable<Account> accounts)
 {
 var provider = _configuration.GetValue<string>("Backend:Provider")?.ToLowerInvariant();
 if (provider == "salesforce")
 {
 // Create records via Salesforce composite or multiple individual requests
 var instanceUrl = _configuration.GetValue<string>("Salesforce:InstanceUrl");
 var results = new List<Account>();
 foreach (var acc in accounts)
 {
 var body = JsonSerializer.Serialize(new { Name = acc.Name, PersonEmail = acc.Email, Description = acc.Address });
 var resp = await _salesforceClient.PostAsync("/services/data/v54.0/sobjects/Account/", body);
 if (!resp.IsSuccessStatusCode)
 {
 _logger.LogWarning("Failed to create SF account: {Status}", resp.StatusCode);
 continue;
 }
 // We won't parse SF response into our Account model reliably; just return input
 results.Add(acc);
 }
 return results;
 }

 using var conn = _dbContext.CreateConnection();
 await conn.OpenAsync();
 using var tx = conn.BeginTransaction();
 try
 {
 var inserted = new List<Account>();
 foreach (var acc in accounts)
 {
 using var cmd = conn.CreateCommand();
 cmd.Transaction = tx;
 cmd.CommandText = "INSERT INTO accounts (name, email, address) VALUES (@name, @email, @address); SELECT LAST_INSERT_ID();";
 var p1 = cmd.CreateParameter();
 p1.ParameterName = "@name";
 p1.Value = (object)acc.Name ?? DBNull.Value;
 cmd.Parameters.Add(p1);
 var p2 = cmd.CreateParameter();
 p2.ParameterName = "@email";
 p2.Value = (object)acc.Email ?? DBNull.Value;
 cmd.Parameters.Add(p2);
 var p3 = cmd.CreateParameter();
 p3.ParameterName = "@address";
 p3.Value = (object)acc.Address ?? DBNull.Value;
 cmd.Parameters.Add(p3);
 var result = await cmd.ExecuteScalarAsync();
 if (result != null && int.TryParse(result.ToString(), out var newId))
 {
 acc.Id = newId;
 }
 inserted.Add(acc);
 cmd.Parameters.Clear();
 }
 tx.Commit();
 return inserted;
 }
 catch
 {
 tx.Rollback();
 throw;
 }
 }

 public async Task<bool> UpdateAsync(int id, Account account)
 {
 var provider = _configuration.GetValue<string>("Backend:Provider")?.ToLowerInvariant();
 if (provider == "salesforce")
 {
 throw new NotSupportedException("Update via numeric id is not supported for Salesforce backend via this method. Use Salesforce endpoints.");
 }

 using var conn = _dbContext.CreateConnection();
 await conn.OpenAsync();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE accounts SET name = @name, email = @email, address = @address WHERE id = @id";
 var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = (object)account.Name ?? DBNull.Value; cmd.Parameters.Add(p1);
 var p2 = cmd.CreateParameter(); p2.ParameterName = "@email"; p2.Value = (object)account.Email ?? DBNull.Value; cmd.Parameters.Add(p2);
 var p3 = cmd.CreateParameter(); p3.ParameterName = "@address"; p3.Value = (object)account.Address ?? DBNull.Value; cmd.Parameters.Add(p3);
 var p4 = cmd.CreateParameter(); p4.ParameterName = "@id"; p4.Value = id; cmd.Parameters.Add(p4);
 var affected = await cmd.ExecuteNonQueryAsync();
 return affected > 0;
 }

 public async Task<bool> DeleteAsync(int id)
 {
 var provider = _configuration.GetValue<string>("Backend:Provider")?.ToLowerInvariant();
 if (provider == "salesforce")
 {
 throw new NotSupportedException("Delete via numeric id is not supported for Salesforce backend via this method. Use Salesforce endpoints.");
 }

 using var conn = _dbContext.CreateConnection();
 await conn.OpenAsync();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = "DELETE FROM accounts WHERE id = @id";
 var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = id; cmd.Parameters.Add(p);
 var affected = await cmd.ExecuteNonQueryAsync();
 return affected > 0;
 }
 }
}
