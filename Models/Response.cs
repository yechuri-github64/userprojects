using System.Collections.Generic;

namespace CreateaccountsdemoLambda.Models
{
    public class ErrorDetail
    {
        public string? Message { get; set; }
        public string? Code { get; set; }
        public string? Details { get; set; }
    }

    public class AccountOutput
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public List<AccountOutput> Accounts { get; set; }
        public ErrorDetail? Error { get; set; }

        public Response()
        {
            Accounts = new List<AccountOutput>();
        }

        public static Response ErrorResponse(string message, string? code = null)
        {
            return new Response
            {
                Success = false,
                Error = new ErrorDetail { Message = message, Code = code }
            };
        }

        public static Response FromException(System.Exception ex)
        {
            return new Response
            {
                Success = false,
                Error = new ErrorDetail { Message = ex.Message, Details = ex.ToString() }
            };
        }
    }
}
