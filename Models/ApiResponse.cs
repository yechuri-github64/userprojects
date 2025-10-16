using System.Text.Json.Serialization;

namespace accounts_sf_sa.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T? data, string message = "") =>
            new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
            };

        public static ApiResponse<T> Fail(string message) =>
            new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
            };
    }
}
