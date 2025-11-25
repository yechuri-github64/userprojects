using System;

namespace TestMyCardLambda.Models
{
    public class Request
    {
        public string? CustomerName { get; set; }
        public decimal TotalAmount { get; set; }

        public Request()
        {
            CustomerName = null!;
            TotalAmount = 0m;
        }
    }
}
