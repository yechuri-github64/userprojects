namespace NumberLambda.Models
{
    public class Response
    {
        // The generated random number (5-100). Null when an error occurs.
        public int? Number { get; set; }

        // Optional success message.
        public string? Message { get; set; }

        // Structured error details when an error occurs.
        public ErrorResponse? Error { get; set; }
    }

    public class ErrorResponse
    {
        // Short error code
        public string Code { get; set; } = string.Empty;

        // Human readable message
        public string Message { get; set; } = string.Empty;

        // Optional detailed information
        public string? Details { get; set; }
    }
}
