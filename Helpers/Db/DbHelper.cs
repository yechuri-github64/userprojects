using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AccountManagerFunctionApp.Helpers.Db
{
 public class DbHelper
 {
 private readonly IConfiguration _configuration;
 private readonly ILogger<DbHelper> _logger;
 private readonly string _connectionString;

 public DbHelper(IConfiguration configuration, ILogger<DbHelper> logger)
 {
 _configuration = configuration;
 _logger = logger;
 _connectionString = _configuration["PostgresConnectionString"] ?? throw new InvalidOperationException("PostgresConnectionString is not configured.");
 }

 public NpgsqlConnection CreateConnection()
 {
 var conn = new NpgsqlConnection(_connectionString);
 return conn;
 }

 public async Task<NpgsqlConnection> CreateOpenConnectionAsync()
 {
 var conn = CreateConnection();
 try
 {
 await conn.OpenAsync();
 return conn;
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to open database connection");
 conn.Dispose();
 throw;
 }
 }
 }
}
