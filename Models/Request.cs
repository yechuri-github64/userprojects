using System;

namespace GetaccountsLambda.Models
{
    public class Request
    {
        public string AccountId { get; set; }

        public Request()
        {
            AccountId = null!;
        }
    }
}
