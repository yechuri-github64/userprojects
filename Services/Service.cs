using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using ContractmanagementemailLambda.Models;
using MySql.Data.MySqlClient;

namespace ContractmanagementemailLambda.Services
{
    public class Service
    {
        private readonly string _conn;
        private static readonly ConcurrentQueue<Response.ResponseItem> _queue = new ConcurrentQueue<Response.ResponseItem>();

        public Service(string conn)
        {
            _conn = conn ?? "dummy";
        }

        /// <summary>
        /// Reads records from the contracts table (id, contractname, email) and enqueues them into an in-memory queue.
        /// If connection string is "dummy" it will enqueue a sample record and skip DB connectivity.
        /// Logger is optional and used for operational logs.
        /// If DB access fails, falls back to enqueuing a dummy record instead of throwing.
        /// </summary>
        public async Task EnqueueFromDbAsync(Action<string>? logger = null)
        {
            try
            {
               context.Logger?.LogInformation("In the function EnqueueFromDbAsync ");

                using var conn = new MySqlConnection(_conn);
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, contractname, email FROM contracts";

                using var reader = await cmd.ExecuteReaderAsync();

                // Get ordinals once
                int idOrd = reader.GetOrdinal("id");
                int cnOrd = reader.GetOrdinal("contractname");
                int emailOrd = reader.GetOrdinal("email");

                while (await reader.ReadAsync())
                {
                    var item = new Response.ResponseItem
                    {
                        Id = reader.IsDBNull(idOrd) ? 0 : reader.GetInt32(idOrd),
                        ContractName = reader.IsDBNull(cnOrd) ? null : reader.GetString(cnOrd),
                        Email = reader.IsDBNull(emailOrd) ? null : reader.GetString(emailOrd)
                    };
                    _queue.Enqueue(item);
                    logger?.Invoke($"Enqueued item from DB: {JsonSerializer.Serialize(item)}");
                    context.Logger?.LogInformation("EnqueueFromDbAsync : Enqueued item from DB");

                }
            }
            catch (Exception ex)
            {
                // If DB access fails, fallback to a dummy item instead of propagating the exception
                logger?.Invoke($"Error in EnqueueFromDbAsync, falling back to dummy item: {ex}");
                var fallback = new Response.ResponseItem
                {
                    Id = 1,
                    ContractName = "SampleContract",
                    Email = "sample@example.com"
                };
                _queue.Enqueue(fallback);
                logger?.Invoke($"Enqueued fallback dummy item: {JsonSerializer.Serialize(fallback)}");
                return;
            }
        }

        /// <summary>
        /// Dequeues a single object from the queue and returns it wrapped in Response.
        /// If queue is empty, returns a structured error in Response.Error.
        /// </summary>
        public Response GetFromQueue()
        {
            try
            {
                if (_queue.TryDequeue(out var item))
                {
                    return new Response { Data = item };
                }

                return new Response
                {
                    Error = new Response.ErrorResponse
                    {
                        Code = "NoData",
                        Message = "Queue is empty",
                        Details = string.Empty
                    }
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Error = new Response.ErrorResponse
                    {
                        Code = "InternalError",
                        Message = ex.Message,
                        Details = ex.ToString()
                    }
                };
            }
        }
    }
}
