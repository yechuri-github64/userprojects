using System;

namespace AccountslamdaLambda.Models
{
    public class Request
    {
        public int AccountId { get; set; }

        public Request()
        {
            AccountId = 0;
        }
    }
}
