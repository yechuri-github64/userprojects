using System;
using System.Threading.Tasks;
using Npgsql;
using TestMyCardLambda.Models;

namespace TestMyCardLambda.Services
{
    public class Service
    {
        private readonly string _connectionString;

        public Service(string connectionString)
        {
            _connectionString = connectionString ?? string.Empty;
        }

        public async Task<Response> CreateOrderAsync(Request request)
        {
            var response = new Response
            {
                CustomerName = request.CustomerName,
                TotalAmount = request.TotalAmount,
                Success = false
            };

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                const string sql = "INSERT INTO orders (customer_name, total_amount) VALUES (@customer_name, @total_amount) RETURNING id, created_at";
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("customer_name", request.CustomerName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("total_amount", request.TotalAmount);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    response.Id = reader.GetInt32(reader.GetOrdinal("id"));
                    response.CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "No rows returned from insert.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
