using System;
using System.Collections.Generic;

namespace AccountslamdaLambda.Models
{
    public class Response
    {
        public bool Success { get; init; }
        public object? Data { get; init; }
        public ErrorResponse? Error { get; init; }

        public Response()
        {
            Success = false;
            Data = null;
            Error = null;
        }
    }

    public class ErrorResponse
    {
        public string? Code { get; init; }
        public string? Message { get; init; }
        public string? Details { get; init; }

        public ErrorResponse()
        {
            Code = null!;
            Message = null!;
            Details = null!;
        }
    }
}
