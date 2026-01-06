using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Npgsql;

namespace accounts_management_c_sharp.Data
{
 public class ApplicationDbContext
 {
 private readonly IConfiguration _configuration;

 public ApplicationDbContext(IConfiguration configuration)
 {
 _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
 }

 public IDbConnection CreateConnection()
 {
 var provider = _configuration.GetValue<string>("Backend:Provider")?.ToLowerInvariant();
 if (provider == "mysql")
 {
 var host = _configuration.GetValue<string>("MySql:Host");
 var port = _configuration.GetValue<int>("MySql:Port");
 var database = _configuration.GetValue<string>("MySql:Database");
 var user = _configuration.GetValue<string>("MySql:User");
 var password = _configuration.GetValue<string>("MySql:Password");
 var cs = new MySqlConnectionStringBuilder
 {
 Server = host,
 Port = (uint)port,
 Database = database,
 UserID = user,
 Password = password,
 AllowUserVariables = true
 };
 return new MySqlConnection(cs.ConnectionString);
 }

 if (provider == "postgresql" || provider == "postgres")
 {
 var host = _configuration.GetValue<string>("Postgres:Host");
 var port = _configuration.GetValue<int>("Postgres:Port");
 var database = _configuration.GetValue<string>("Postgres:Database");
 var user = _configuration.GetValue<string>("Postgres:User");
 var password = _configuration.GetValue<string>("Postgres:Password");
 var cs = $"Host={host};Port={port};Database={database};Username={user};Password={password}";
 return new NpgsqlConnection(cs);
 }

 throw new InvalidOperationException("Unsupported database provider configured. Please set Backend:Provider to mysql or postgresql.");
 }
 }
}
