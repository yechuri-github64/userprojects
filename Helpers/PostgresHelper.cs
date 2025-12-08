using System.Data;
using Npgsql;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace SalesforceAccountFunctions.Helpers
{
 public class PostgresOptions
 {
 public string ConnectionString { get; set; } = string.Empty;
 }

 public interface ILoggingService
 {
 Task LogAsync(string category, string action, string details);
 Task LogErrorAsync(string category, string action, string details);
 }

 public class LoggingService : ILoggingService
 {
 private readonly PostgresOptions _options;
 private readonly ILogger<LoggingService> _logger;

 public LoggingService(IOptions<PostgresOptions> options, ILogger<LoggingService> logger)
 {
 _options = options.Value;
 _logger = logger;

 // Ensure table exists
 try
 {
 using var conn = new NpgsqlConnection(_options.ConnectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();
 cmd.CommandText = @"
 CREATE TABLE IF NOT EXISTS function_logs (
 id SERIAL PRIMARY KEY,
 category TEXT,
 action TEXT,
 details TEXT,
 created_at TIMESTAMP WITH TIME ZONE DEFAULT now()
 );";
 cmd.ExecuteNonQuery();
 }
 catch (Exception ex)
 {
 _logger.LogWarning(ex, "Could not ensure logs table exists");
 }
 }

 public async Task LogAsync(string category, string action, string details)
 {
 try
 {
 await using var conn = new NpgsqlConnection(_options.ConnectionString);
 await conn.OpenAsync();
 await using var cmd = conn.CreateCommand();
 cmd.CommandText = "INSERT INTO function_logs(category, action, details) VALUES(@c, @a, @d);";
 cmd.Parameters.AddWithValue("@c", category ?? string.Empty);
 cmd.Parameters.AddWithValue("@a", action ?? string.Empty);
 cmd.Parameters.AddWithValue("@d", details ?? string.Empty);
 await cmd.ExecuteNonQueryAsync();
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to log to Postgres");
 }
 }

 public Task LogErrorAsync(string category, string action, string details)
 {
 return LogAsync(category, action, details);
 }
 }
}
