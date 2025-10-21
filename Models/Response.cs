using System;

namespace NumberLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public int? Number { get; set; }
        public ErrorDetail? Error { get; set; }

        public class ErrorDetail
        {
            public string Code { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string? Details { get; set; }
        }
    }
}