using System.Text.Json;

namespace ManageordersLambda.Models
{
    public class ErrorModel
    {
        public ErrorModel()
        {
            Code = null!;
            Message = null!;
            Details = null;
        }

        public string? Code { get; set; }
        public string? Message { get; set; }
        public JsonElement? Details { get; set; }
    }

    public class Response
    {
        public Response()
        {
            Success = false;
            Data = null;
            Error = null;
        }

        public bool Success { get; set; }
        public JsonElement? Data { get; set; }
        public ErrorModel? Error { get; set; }
    }
}
