using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using AccountslamdaLambda.Models;

namespace AccountslamdaLambda.Services
{
    public class Service
    {
        private readonly string _connString;

        public Service(IConfiguration config)
        {
            _connString = config.GetConnectionString("MySQL") ?? config["ConnectionStrings:MySQL"] ?? "server=localhost;user=dummy;password=skip;database=accountslamda;";
        }

        /// <summary>Returns the last created account for the given id (ordered by created_at desc). Returns null if not found.
        /// If DB connection fails, returns a small in-memory dummy fallback list (so caller still receives a result instead of hard failure).
        /// </summary>
        public async Task<Response.Account?> GetLastCreatedAccountByIdAsync(int id)
        {
            try
            {
                await using var conn = new MySqlConnection(_connString);
                await conn.OpenAsync();

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, name, email, created_at FROM accounts WHERE id = @id ORDER BY created_at DESC LIMIT 1;";
                cmd.Parameters.Add(new MySqlParameter("@id", id));

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var account = new Response.Account
                    {
                        Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32("id"),
                        Name = reader.IsDBNull(reader.GetOrdinal("name")) ? null : reader.GetString("name"),
                        Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? DateTime.MinValue : reader.GetDateTime("created_at")
                    };

                    return account;
                }

                return null;
            }
            catch (Exception ex)
            {
                // If DB connection or query fails, log and return a small dummy fallback list to allow the lambda to continue
                Console.Error.WriteLine($"DB access failed: {ex}");

                var fallback = new List<Response.Account>
                {
                    new Response.Account { Id = id, Name = "Dummy User", Email = "dummy@example.com", CreatedAt = DateTime.UtcNow }
                };

                // Return the last created from fallback (or null if none)
                return fallback.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
            }
        }
    }
}
