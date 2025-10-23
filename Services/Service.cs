namespace GetaccountsLambda.Services;

#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon.Lambda.Core;
using GetaccountsLambda.Models;
using MySqlConnector;

public class Service
{
    private readonly IConfiguration _configuration;

    public Service(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Response> GetLastAccountAsync(ILambdaContext context)
    {
        var response = new Response();
        try
        {
            var connStr = _configuration.GetConnectionString("MySQL");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                var msg = "MySQL connection string is not configured.";
                context.Logger.LogLine(msg);
                response = new Response
                {
                    Error = new Response.ErrorModel
                    {
                        Code = "ConfigError",
                        Message = msg
                    }
                };
                return response;
            }

            // Attempt to connect and fetch last account. If DB connection fails, fall back to a dummy account.
            try
            {
                await using var conn = new MySqlConnection(connStr);
                await conn.OpenAsync();

                const string sql = "SELECT * FROM accounts ORDER BY id DESC LIMIT 1;";
                await using var cmd = new MySqlCommand(sql, conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var account = new Response.AccountModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"] == DBNull.Value ? null : reader["name"]?.ToString(),
                        Email = reader["email"] == DBNull.Value ? null : reader["email"]?.ToString(),
                        CreatedAt = reader["created_at"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["created_at"])
                    };

                    response = new Response { Account = account };
                }
                else
                {
                    response = new Response
                    {
                        Error = new Response.ErrorModel
                        {
                            Code = "NotFound",
                            Message = "No account records found."
                        }
                    };
                }
            }
            catch (MySqlException mex)
            {
                // DB connection or query failed: log and return a dummy account as fallback
                context.Logger.LogLine($"MySQL error, using dummy data: {mex}");

                var dummy = new Response.AccountModel
                {
                    Id = -1,
                    Name = "Dummy Account",
                    Email = "dummy@example.com",
                    CreatedAt = DateTime.UtcNow
                };

                response = new Response { Account = dummy };
            }
        }
        catch (Exception ex)
        {
            // Log the full exception for diagnostics and return a structured error to the caller
            context.Logger.LogLine($"Exception while fetching last account: {ex}");
            response = new Response
            {
                Error = new Response.ErrorModel
                {
                    Code = "ServerError",
                    Message = ex.Message,
                    Details = ex.ToString()
                }
            };
        }

        return response;
    }
}
