using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManagerFunctionApp.Models;
using AccountManagerFunctionApp.Helpers.Db;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AccountManagerFunctionApp.Repositories
{
 public class AccountRepository : IAccountRepository
 {
 private readonly DbHelper _dbHelper;
 private readonly ILogger<AccountRepository> _logger;

 public AccountRepository(DbHelper dbHelper, ILogger<AccountRepository> logger)
 {
 _dbHelper = dbHelper;
 _logger = logger;
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 var results = new List<Account>();
 await using var conn = await _dbHelper.CreateOpenConnectionAsync();
 const string sql = "SELECT id::text, name, email, address FROM accounts";
 await using var cmd = new NpgsqlCommand(sql, conn);
 try
 {
 await using var reader = await cmd.ExecuteReaderAsync();
 while (await reader.ReadAsync())
 {
 results.Add(new Account
 {
 Id = reader.GetString(0),
 Name = reader.IsDBNull(1) ? null : reader.GetString(1),
 Email = reader.IsDBNull(2) ? null : reader.GetString(2),
 Address = reader.IsDBNull(3) ? null : reader.GetString(3)
 });
 }
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error fetching all accounts");
 throw;
 }

 return results;
 }

 public async Task<Account?> GetByIdAsync(Guid id)
 {
 await using var conn = await _dbHelper.CreateOpenConnectionAsync();
 const string sql = "SELECT id::text, name, email, address FROM accounts WHERE id = @id";
 await using var cmd = new NpgsqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@id", id);
 try
 {
 await using var reader = await cmd.ExecuteReaderAsync();
 if (await reader.ReadAsync())
 {
 return new Account
 {
 Id = reader.GetString(0),
 Name = reader.IsDBNull(1) ? null : reader.GetString(1),
 Email = reader.IsDBNull(2) ? null : reader.GetString(2),
 Address = reader.IsDBNull(3) ? null : reader.GetString(3)
 };
 }

 return null;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error fetching account by id {Id}", id);
 throw;
 }
 }

 public async Task<IEnumerable<Account>> CreateBatchAsync(IEnumerable<Account> accounts)
 {
 var created = new List<Account>();
 await using var conn = await _dbHelper.CreateOpenConnectionAsync();
 await using var tx = await conn.BeginTransactionAsync();
 try
 {
 const string sql = "INSERT INTO accounts (id, name, email, address) VALUES (@id, @name, @email, @address)";
 await using var cmd = new NpgsqlCommand(sql, conn, tx);
 foreach (var acc in accounts)
 {
 var id = Guid.NewGuid();
 if (!string.IsNullOrWhiteSpace(acc.Id) && Guid.TryParse(acc.Id, out var parsed))
 {
 id = parsed;
 }

 cmd.Parameters.Clear();
 cmd.Parameters.AddWithValue("@id", id);
 cmd.Parameters.AddWithValue("@name", (object?)acc.Name ?? DBNull.Value);
 cmd.Parameters.AddWithValue("@email", (object?)acc.Email ?? DBNull.Value);
 cmd.Parameters.AddWithValue("@address", (object?)acc.Address ?? DBNull.Value);
 await cmd.ExecuteNonQueryAsync();

 created.Add(new Account
 {
 Id = id.ToString(),
 Name = acc.Name,
 Email = acc.Email,
 Address = acc.Address
 });
 }

 await tx.CommitAsync();
 }
 catch (Exception ex)
 {
 await tx.RollbackAsync();
 _logger.LogError(ex, "Error creating batch accounts");
 throw;
 }

 return created;
 }

 public async Task<Account?> UpdateAsync(Guid id, Account updated)
 {
 await using var conn = await _dbHelper.CreateOpenConnectionAsync();
 const string sql = "UPDATE accounts SET name = @name, email = @email, address = @address WHERE id = @id";
 await using var cmd = new NpgsqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@id", id);
 cmd.Parameters.AddWithValue("@name", (object?)updated.Name ?? DBNull.Value);
 cmd.Parameters.AddWithValue("@email", (object?)updated.Email ?? DBNull.Value);
 cmd.Parameters.AddWithValue("@address", (object?)updated.Address ?? DBNull.Value);
 try
 {
 var rows = await cmd.ExecuteNonQueryAsync();
 if (rows == 0)
 return null;

 return await GetByIdAsync(id);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error updating account {Id}", id);
 throw;
 }
 }

 public async Task<bool> DeleteAsync(Guid id)
 {
 await using var conn = await _dbHelper.CreateOpenConnectionAsync();
 const string sql = "DELETE FROM accounts WHERE id = @id";
 await using var cmd = new NpgsqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@id", id);
 try
 {
 var rows = await cmd.ExecuteNonQueryAsync();
 return rows > 0;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error deleting account {Id}", id);
 throw;
 }
 }
 }
}
