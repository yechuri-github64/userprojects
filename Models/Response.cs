namespace TestSfLambdaLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public ErrorDetail? Error { get; set; }

        public Response(bool success, object? data, ErrorDetail? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }
    }

    public class ErrorDetail
    {
        public string? Code { get; set; }
        public string? Message { get; set; }

        public ErrorDetail(string? code, string? message)
        {
            Code = code;
            Message = message;
        }
    }
}
