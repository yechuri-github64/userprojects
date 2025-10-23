using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using AccountslamdaLambda.Models;

namespace AccountslamdaLambda.Services
{
    public class Service
    {
        private readonly string _connectionString;

        public Service(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySql") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the last created account row (by created_at) for a given account id from the accounts table.
        /// Returns null if none found.
        /// </summary>
        public async Task<Response.Account?> GetLastCreatedAccountAsync(int accountId)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("MySql connection string is not configured.");
            }

            const string sql = @"
SELECT id, account_id, name, email, created_at
FROM accounts
WHERE account_id = @accountId
ORDER BY created_at DESC
LIMIT 1;";

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@accountId", accountId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            var account = new Response.Account
            {
                Id = reader.GetInt32("id"),
                AccountId = reader.GetInt32("account_id"),
                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString("name"),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString("email"),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.MinValue : reader.GetDateTime("created_at")
            };

            return account;
        }
    }
}
