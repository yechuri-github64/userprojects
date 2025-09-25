using System;
using System.IO;
using System.Text.Json;

namespace Test_project_mainLambda
{
    // Represents the structure of the provided get.json file used for connector configuration
    public class ConnectorConfig
    {
        public string? Queryparameters { get; set; }
        public string? Headerparameters { get; set; }
        public InputSection? Input { get; set; }
        public OutputSection? Output { get; set; }

        public static ConnectorConfig? LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path is null or empty", nameof(path));
            if (!File.Exists(path)) return null;
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ConnectorConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    public class InputSection
    {
        public string? description { get; set; }
        public object? queryParameters { get; set; }
    }

    public class OutputSection
    {
        public string? contentType { get; set; }
        public object? body { get; set; }
        public object? example { get; set; }
    }
}
