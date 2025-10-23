using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using AccountslamdaLambda.Models;
using Amazon.Lambda.Core;

namespace AccountslamdaLambda.Services
{
    public class Service
    {
        private readonly string _connectionString;

        public Service()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("MySQL") ?? string.Empty;
        }

        public async Task<object?> GetLastAccountByIdAsync(int id, ILambdaContext? context = null)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                context?.Logger.LogLine("Connection string 'MySQL' is not configured.");
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            try
            {
                await using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync();

                const string sql = "SELECT * FROM accounts WHERE id = @id ORDER BY created_at DESC LIMIT 1;";

                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    context?.Logger.LogLine($"No account found for id {id}.");
                    return null;
                }

                var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                if (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var value = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                        result[name] = value;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"Database error: {ex}");
                throw;
            }
        }
    }
}
