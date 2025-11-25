using System;

namespace TestMyCardLambda.Models
{
    public class Response
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public Response()
        {
            CustomerName = null!;
            ErrorMessage = null!;
            CreatedAt = DateTime.MinValue;
            Success = false;
        }
    }
}
