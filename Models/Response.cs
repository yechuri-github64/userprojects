using System.Text.Json.Serialization;

namespace TestsflambdaLambda.Models
{
    public class Response
    {
        public Response()
        {
            Success = false;
            Message = string.Empty;
            Data = null;
            Error = null;
        }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("error")]
        public ErrorResponse? Error { get; set; }

        public static Response Ok(object? data)
        {
            return new Response { Success = true, Message = "OK", Data = data };
        }

        public static Response Error(string message, string details)
        {
            return new Response
            {
                Success = false,
                Message = message,
                Error = new ErrorResponse { Code = 400, Message = message, Details = details }
            };
        }

        public static Response FromException(ServiceException ex)
        {
            return new Response
            {
                Success = false,
                Message = ex.Message,
                Error = new ErrorResponse { Code = ex.StatusCode, Message = ex.Message, Details = ex.Details }
            };
        }
    }

    public class ErrorResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }

    public class ServiceException : System.Exception
    {
        public int StatusCode { get; }
        public string? Details { get; }

        public ServiceException(int statusCode, string message, string? details = null) : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }
    }
}
