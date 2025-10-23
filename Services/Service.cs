using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using KailashcontractslambdaLambda.Models;
using MySql.Data.MySqlClient;

namespace KailashcontractslambdaLambda.Services
{
    public class Service
    {
        private readonly string _connectionString;

        public Service()
        {
            // Load connection string from appsettings.json
            try
            {
                var file = "appsettings.json";
                if (File.Exists(file))
                {
                    var text = File.ReadAllText(file);
                    using var doc = JsonDocument.Parse(text);
                    if (doc.RootElement.TryGetProperty("ConnectionStrings", out var cs) && cs.ValueKind == JsonValueKind.Object)
                    {
                        if (cs.TryGetProperty("MySQL", out var mysql))
                        {
                            _connectionString = mysql.GetString() ?? "Server=dummy;Uid=dummy;Pwd=dummy;Database=contractsdb;";
                        }
                        else
                        {
                            _connectionString = "Server=dummy;Uid=dummy;Pwd=dummy;Database=contractsdb;";
                        }
                    }
                    else
                    {
                        _connectionString = "Server=dummy;Uid=dummy;Pwd=dummy;Database=contractsdb;";
                    }
                }
                else
                {
                    _connectionString = "Server=dummy;Uid=dummy;Pwd=dummy;Database=contractsdb;";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error reading appsettings.json: " + ex.ToString());
                _connectionString = "Server=dummy;Uid=dummy;Pwd=dummy;Database=contractsdb;";
            }
        }

        public async Task<Response> GetContractWithHighestIdAsync()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, contractname, email FROM contracts ORDER BY id DESC LIMIT 1;";

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    int id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    string? contractName = reader.IsDBNull(1) ? null : reader.GetString(1);
                    string? email = reader.IsDBNull(2) ? null : reader.GetString(2);

                    return new Response
                    {
                        Id = id,
                        ContractName = contractName,
                        Email = email
                    };
                }

                return new Response
                {
                    Error = new ErrorDetail
                    {
                        Code = "NotFound",
                        Message = "No contracts found in the contracts table."
                    }
                };
            }
            catch (Exception ex)
            {
                // Log the full exception and return structured error
                Console.Error.WriteLine(ex.ToString());
                return new Response
                {
                    Error = new ErrorDetail
                    {
                        Code = "DbError",
                        Message = ex.Message
                    }
                };
            }
        }
    }
}
