using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using MySql.Data.MySqlClient;

namespace Test_project_mainLambda
{
    public static class ConnectorHelper
    {
        // Attempts to validate a MySQL connection using either an environment variable or connector config file
        public static async Task<ConnectorResult> ValidateMySqlConnectionAsync(ILambdaContext? context = null)
        {
            try
            {
                string? conn = Environment.GetEnvironmentVariable("MYSQL_CONNECTION");

                // Try to load get.json if present and if no env var provided
                if (string.IsNullOrEmpty(conn))
                {
                    var config = ConnectorConfig.LoadFromFile("get.json");
                    // If the config file contains connection details in some custom place, extract here.
                    // For safety, we don't assume any specific field. So we return not attempted.
                    if (config == null)
                    {
                        return new ConnectorResult { Success = false, Message = "No MYSQL_CONNECTION env var and get.json not found." };
                    }
                }

                if (string.IsNullOrEmpty(conn))
                {
                    return new ConnectorResult { Success = false, Message = "No MySQL connection string provided." };
                }

                using var connection = new MySqlConnection(conn);
                await connection.OpenAsync();
                await connection.CloseAsync();

                return new ConnectorResult { Success = true, Message = "Connection successful." };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"ConnectorHelper error: {ex.Message}");
                return new ConnectorResult { Success = false, Message = ex.Message, Exception = ex };
            }
        }
    }

    public class ConnectorResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        [JsonIgnore]
        public Exception? Exception { get; set; }
    }
}
