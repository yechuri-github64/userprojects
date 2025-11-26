using System;
using System.Collections.Generic;

namespace CreateaccountsdemoLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public ErrorModel? Error { get; set; }
    }

    public class ErrorModel
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CreateResult
    {
        public string? Id { get; set; }
        public bool Success { get; set; }
        public string? Errors { get; set; }
    }
}
