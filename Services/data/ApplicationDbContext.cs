using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using account_c_sharp.Models;
using MySqlConnector;

namespace account_c_sharp.Services
{
 public class ApplicationDbContext
 {
 private readonly string _connectionString;
 private readonly ILogger<ApplicationDbContext> _logger;

 public ApplicationDbContext(IConfiguration configuration, ILogger<ApplicationDbContext> logger)
 {
 _logger = logger;
 var provider = configuration.GetValue<string>("Connection:Provider");
 var host = configuration.GetValue<string>("Connection:Host");
 var port = configuration.GetValue<int>("Connection:Port");
 var database = configuration.GetValue<string>("Connection:Database");
 var user = configuration.GetValue<string>("Connection:User");
 var password = configuration.GetValue<string>("Connection:Password");

 if (string.IsNullOrWhiteSpace(provider) || provider.ToLowerInvariant() != "mysql")
 {
 _logger.LogWarning("Provider is not set to mysql. Current provider: {Provider}", provider);
 }

 _connectionString = new MySqlConnectionStringBuilder
 {
 Server = host ?? "localhost",
 Port = (uint)(port <= 0 ? 3306 : port),
 Database = database ?? "",
 UserID = user ?? "",
 Password = password ?? "",
 AllowUserVariables = true,
 SslMode = MySqlSslMode.None
 }.ConnectionString;

 try
 {
 EnsureTableAsync().GetAwaiter().GetResult();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to ensure accounts table exists");
 throw;
 }
 }

 private async Task EnsureTableAsync()
 {
 var sql = @"
CREATE TABLE IF NOT EXISTS accounts (
 id INT PRIMARY KEY AUTO_INCREMENT,
 name VARCHAR(255) NOT NULL,
 email VARCHAR(255) NOT NULL,
 address TEXT
) ENGINE=InnoDB;";

 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var cmd = new MySqlCommand(sql, conn);
 await cmd.ExecuteNonQueryAsync();
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 var list = new List<Account>();
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();

 await using var cmd = new MySqlCommand("SELECT id, name, email, address FROM accounts", conn);
 await using var reader = await cmd.ExecuteReaderAsync();
 while (await reader.ReadAsync())
 {
 list.Add(new Account
 {
 Id = reader.GetInt32(0),
 Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
 Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
 Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
 });
 }

 return list;
 }

 public async Task<Account> GetByIdAsync(int id)
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();

 await using var cmd = new MySqlCommand("SELECT id, name, email, address FROM accounts WHERE id = @id", conn);
 cmd.Parameters.AddWithValue("@id", id);

 await using var reader = await cmd.ExecuteReaderAsync();
 if (await reader.ReadAsync())
 {
 return new Account
 {
 Id = reader.GetInt32(0),
 Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
 Email = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
 Address = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
 };
 }

 return null;
 }

 public async Task<IEnumerable<Account>> CreateManyAsync(IEnumerable<Account> accounts)
 {
 var created = new List<Account>();
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();
 await using var tran = await conn.BeginTransactionAsync();

 try
 {
 foreach (var acc in accounts)
 {
 if (string.IsNullOrWhiteSpace(acc.Name) || string.IsNullOrWhiteSpace(acc.Email))
 throw new ArgumentException("Each account must have at least a name and an email.");

 await using var cmd = new MySqlCommand("INSERT INTO accounts (name, email, address) VALUES (@name, @email, @address)", conn, tran);
 cmd.Parameters.AddWithValue("@name", acc.Name);
 cmd.Parameters.AddWithValue("@email", acc.Email);
 cmd.Parameters.AddWithValue("@address", acc.Address ?? string.Empty);

 await cmd.ExecuteNonQueryAsync();
 var lastId = (int)cmd.LastInsertedId;
 created.Add(new Account
 {
 Id = lastId,
 Name = acc.Name,
 Email = acc.Email,
 Address = acc.Address
 });
 }

 await tran.CommitAsync();
 return created;
 }
 catch (Exception)
 {
 try
 {
 await tran.RollbackAsync();
 }
 catch (Exception rollbackEx)
 {
 _logger.LogError(rollbackEx, "Rollback failed after CreateManyAsync exception");
 }

 _logger.LogError("CreateManyAsync failed and was rolled back");
 throw;
 }
 }

 public async Task<bool> UpdateAsync(Account account)
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();

 await using var cmd = new MySqlCommand("UPDATE accounts SET name = @name, email = @email, address = @address WHERE id = @id", conn);
 cmd.Parameters.AddWithValue("@name", account.Name ?? string.Empty);
 cmd.Parameters.AddWithValue("@email", account.Email ?? string.Empty);
 cmd.Parameters.AddWithValue("@address", account.Address ?? string.Empty);
 cmd.Parameters.AddWithValue("@id", account.Id);

 var affected = await cmd.ExecuteNonQueryAsync();
 return affected > 0;
 }

 public async Task<bool> DeleteAsync(int id)
 {
 await using var conn = new MySqlConnection(_connectionString);
 await conn.OpenAsync();

 await using var cmd = new MySqlCommand("DELETE FROM accounts WHERE id = @id", conn);
 cmd.Parameters.AddWithValue("@id", id);
 var affected = await cmd.ExecuteNonQueryAsync();
 return affected > 0;
 }
 }
}
