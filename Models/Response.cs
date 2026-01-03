using System;
using System.Collections.Generic;

namespace TestLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public int? TeamId { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ErrorResponse
    {
        public ErrorResponse(IEnumerable<string> errs)
        {
            Errors = new List<string>(errs);
            Success = false;
        }

        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}
