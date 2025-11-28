using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Services
{
 public class AccountService : IAccountService
 {
 private readonly string _connectionString;
 private readonly ILogger<AccountService> _logger;

 public AccountService(IConfiguration configuration, ILogger<AccountService> logger)
 {
 _logger = logger;
 var host = configuration["MySqlHost"] ?? "localhost";
 var portStr = configuration["MySqlPort"] ?? "3306";
 var user = configuration["MySqlUser"] ?? "root";
 var password = configuration["MySqlPassword"] ?? string.Empty;
 var database = configuration["MySqlDatabase"] ?? "accountsdb";

 if (!uint.TryParse(portStr, out var port))
 {
 port = 3306;
 }

 var builder = new MySqlConnectionStringBuilder()
 {
 Server = host,
 Port = port,
 UserID = user,
 Password = password,
 Database = database,
 SslMode = MySqlSslMode.None
 };

 _connectionString = builder.ConnectionString;

 try
 {
 // Ensure table exists. Synchronously wait in constructor to avoid missing table at runtime.
 using var conn = new MySqlConnection(_connectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS accounts (
 id VARCHAR(36) PRIMARY KEY,
 name VARCHAR(255),
 email VARCHAR(255),
 address TEXT
 );";
 cmd.ExecuteNonQuery();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to ensure accounts table exists");
 }
 }

 public async Task<List<Account>> CreateAccountsAsync(List<Account> accounts)
 {
 if (accounts == null) throw new ArgumentNullException(nameof(accounts));
 var created = new List<Account>();
 try
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var tran = await conn.BeginTransactionAsync();

 foreach (var account in accounts)
 {
 if (string.IsNullOrWhiteSpace(account.Id)) account.Id = Guid.NewGuid().ToString();
 await using var cmd = conn.CreateCommand();
 cmd.Transaction = tran;
 cmd.CommandText = @"INSERT INTO accounts (id, name, email, address) VALUES (@id, @name, @email, @address);";
 cmd.Parameters.AddWithValue("@id", account.Id);
 cmd.Parameters.AddWithValue("@name", account.Name ?? string.Empty);
 cmd.Parameters.AddWithValue("@email", account.Email ?? string.Empty);
 cmd.Parameters.AddWithValue("@address", account.Address ?? string.Empty);
 await cmd.ExecuteNonQueryAsync();
 created.Add(account);
 }

 await tran.CommitAsync();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error creating accounts");
 throw;
 }

 return created;
 }

 public async Task<Account?> GetAccountAsync(string id)
 {
 try
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var cmd = conn.CreateCommand();
 cmd.CommandText = @"SELECT id, name, email, address FROM accounts WHERE id = @id LIMIT 1;";
 cmd.Parameters.AddWithValue("@id", id);
 await using var reader = await cmd.ExecuteReaderAsync();
 if (await reader.ReadAsync())
 {
 return new Account
 {
 Id = reader.GetString(0),
 Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
 Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
 Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
 };
 }
 return null;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error retrieving account {Id}", id);
 throw;
 }
 }

 public async Task<Account?> UpdateAccountAsync(string id, Account account)
 {
 try
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var cmd = conn.CreateCommand();
 cmd.CommandText = @"UPDATE accounts SET name = @name, email = @email, address = @address WHERE id = @id;";
 cmd.Parameters.AddWithValue("@id", id);
 cmd.Parameters.AddWithValue("@name", account.Name ?? string.Empty);
 cmd.Parameters.AddWithValue("@email", account.Email ?? string.Empty);
 cmd.Parameters.AddWithValue("@address", account.Address ?? string.Empty);
 var rows = await cmd.ExecuteNonQueryAsync();
 if (rows == 0) return null;
 return await GetAccountAsync(id);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {Id}", id);
 throw;
 }
 }

 public async Task<bool> DeleteAccountAsync(string id)
 {
 try
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var cmd = conn.CreateCommand();
 cmd.CommandText = @"DELETE FROM accounts WHERE id = @id;";
 cmd.Parameters.AddWithValue("@id", id);
 var rows = await cmd.ExecuteNonQueryAsync();
 return rows > 0;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 throw;
 }
 }

 public async Task<List<Account>> ListAccountsAsync()
 {
 var list = new List<Account>();
 try
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var cmd = conn.CreateCommand();
 cmd.CommandText = @"SELECT id, name, email, address FROM accounts;";
 await using var reader = await cmd.ExecuteReaderAsync();
 while (await reader.ReadAsync())
 {
 list.Add(new Account
 {
 Id = reader.GetString(0),
 Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
 Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
 Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
 });
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error listing accounts");
 throw;
 }

 return list;
 }
 }
}