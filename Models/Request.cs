using System;

namespace DatatableLambda.Models
{
    /// <summary>
    /// Request model for retrieving accounts. Supports basic pagination.
    /// </summary>
    public class Request
    {
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
