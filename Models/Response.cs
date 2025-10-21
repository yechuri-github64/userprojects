using System.Collections.Generic;
using DatatableLambda.Data;

namespace DatatableLambda.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<Account>? Accounts { get; set; }
    }
}
