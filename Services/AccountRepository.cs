using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using account-mangement.Models;

namespace account-mangement.Services
{
 public class AccountRepository
 {
 private readonly ApplicationDbContext _context;

 public AccountRepository(ApplicationDbContext context)
 {
 _context = context;
 }

 public async Task<IEnumerable<Account>> GetAllAsync()
 {
 var results = new List<Account>();
 await using var conn = _context.CreateConnection();
 await conn.OpenAsync();

 await using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT id, name, email, address FROM accounts";

 await using var reader = await cmd.ExecuteReaderAsync();
 while (await reader.ReadAsync())
 {
 results.Add(new Account
 {
 Id = reader.GetInt32(0),
 Name = reader.IsDBNull(1) ? null : reader.GetString(1),
 Email = reader.IsDBNull(2) ? null : reader.GetString(2),
 Address = reader.IsDBNull(3) ? null : reader.GetString(3)
 });
 }

 return results;
 }

 public async Task<Account> GetByIdAsync(int id)
 {
 await using var conn = _context.CreateConnection();
 await conn.OpenAsync();

 await using var cmd = conn.CreateCommand();
 cmd.CommandText = "SELECT id, name, email, address FROM accounts WHERE id = @id";
 cmd.Parameters.AddWithValue("@id", id);

 await using var reader = await cmd.ExecuteReaderAsync();
 if (!await reader.ReadAsync()) return null;

 return new Account
 {
 Id = reader.GetInt32(0),
 Name = reader.IsDBNull(1) ? null : reader.GetString(1),
 Email = reader.IsDBNull(2) ? null : reader.GetString(2),
 Address = reader.IsDBNull(3) ? null : reader.GetString(3)
 };
 }

 public async Task<IEnumerable<Account>> CreateMultipleAsync(IEnumerable<Account> accounts)
 {
 var created = new List<Account>();

 await using var conn = _context.CreateConnection();
 await conn.OpenAsync();

 await using var transaction = await conn.BeginTransactionAsync();
 try
 {
 foreach (var acc in accounts)
 {
 if (acc == null) throw new ArgumentException("Account entry cannot be null.");

 await using var cmd = conn.CreateCommand();
 cmd.Transaction = transaction;
 cmd.CommandText = "INSERT INTO accounts (name, email, address) VALUES (@name, @email, @address); SELECT LAST_INSERT_ID();";
 cmd.Parameters.AddWithValue("@name", acc.Name ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@email", acc.Email ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@address", acc.Address ?? (object)DBNull.Value);

 var idObj = await cmd.ExecuteScalarAsync();
 var id = Convert.ToInt32(idObj);

 created.Add(new Account
 {
 Id = id,
 Name = acc.Name,
 Email = acc.Email,
 Address = acc.Address
 });
 }

 await transaction.CommitAsync();
 return created;
 }
 catch
 {
 await transaction.RollbackAsync();
 throw;
 }
 }

 public async Task<bool> UpdateAsync(Account account)
 {
 await using var conn = _context.CreateConnection();
 await conn.OpenAsync();

 await using var cmd = conn.CreateCommand();
 cmd.CommandText = "UPDATE accounts SET name = @name, email = @email, address = @address WHERE id = @id";
 cmd.Parameters.AddWithValue("@name", account.Name ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@email", account.Email ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@address", account.Address ?? (object)DBNull.Value);
 cmd.Parameters.AddWithValue("@id", account.Id);

 var affected = await cmd.ExecuteNonQueryAsync();
 return affected > 0;
 }
 }
}
