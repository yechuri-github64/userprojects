using System;

namespace AccountslamdaLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public Account? Data { get; set; }
        public ErrorDetail? Error { get; set; }

        public Response()
        {
            Success = false;
            Data = null;
            Error = null;
        }

        public class Account
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public DateTime CreatedAt { get; set; }

            public Account()
            {
                Id = 0;
                Name = null;
                Email = null;
                CreatedAt = DateTime.MinValue;
            }
        }

        public class ErrorDetail
        {
            public string Message { get; set; }
            public string? Details { get; set; }
            public string? StackTrace { get; set; }

            public ErrorDetail()
            {
                Message = string.Empty;
                Details = null;
                StackTrace = null;
            }
        }
    }
}
