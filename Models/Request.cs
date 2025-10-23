using System;

namespace ContractmanagementemailLambda.Models
{
    public class Request
    {
        public string? Action { get; set; }
        public int? Count { get; set; }

        public Request()
        {
            Action = null!;
            Count = null;
        }
    }
}
