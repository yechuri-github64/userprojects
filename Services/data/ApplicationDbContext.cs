using System;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace account-mangement.Services
{
 public class ApplicationDbContext
 {
 private readonly string _connectionString;

 public ApplicationDbContext(IConfiguration configuration)
 {
 // Build connection string from configuration values (provider and port included in config)
 var provider = configuration["Database:Provider"] ?? configuration["Database:provider"] ?? "MySqlConnector";
 var host = configuration["Database:Host"] ?? "localhost";
 var port = configuration["Database:Port"] ?? configuration["Database:port"] ?? "3306";
 var database = configuration["Database:Database"] ?? configuration["Database:database"] ?? "accounts_db";
 var user = configuration["Database:User"] ?? configuration["Database:user"] ?? "root";
 var password = configuration["Database:Password"] ?? configuration["Database:password"] ?? "";

 // Note: provider is kept for future extensibility; currently we use MySqlConnector directly.
 _connectionString = $"Server={host};Port={port};Database={database};User={user};Password={password};";
 }

 public MySqlConnection CreateConnection()
 {
 return new MySqlConnection(_connectionString);
 }
 }
}
