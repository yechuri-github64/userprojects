using System;
using System.Collections.Generic;
using DatatableLambda.Data;

namespace DatatableLambda.Models
{
    public class ErrorDetail
    {
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Account>? Data { get; set; }
        public ErrorDetail? Error { get; set; }
    }
}
