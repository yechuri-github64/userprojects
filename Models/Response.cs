using System;

namespace AccountslamdaLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public Account? Data { get; set; }
        public ErrorResponse? Error { get; set; }

        public Response()
        {
            Success = false;
            Data = null;
            Error = null;
        }

        public class Account
        {
            public int Id { get; set; }
            public int AccountId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; }

            public Account()
            {
                Name = string.Empty;
                Email = string.Empty;
                CreatedAt = DateTime.MinValue;
            }
        }

        public class ErrorResponse
        {
            public string Message { get; set; }
            public string? Details { get; set; }

            public ErrorResponse()
            {
                Message = string.Empty;
                Details = null;
            }
        }
    }
}
