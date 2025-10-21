using System;
using System.Collections.Generic;

namespace DatatableLambda.Models
{
    /// <summary>
    /// Standard response returned by the datatable lambda.
    /// Contains Success flag, Data (list of accounts) and structured Error when applicable.
    /// </summary>
    public class Response
    {
        public bool Success { get; set; }
        public List<AccountDto> Data { get; set; } = new List<AccountDto>();
        public ErrorInfo? Error { get; set; }

        public class AccountDto
        {
            public int Id { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class ErrorInfo
        {
            public string? Code { get; set; }
            public string? Message { get; set; }
            public string? Details { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
