using System.Collections.Generic;

namespace GetaccountsLambda.Models
{
    public class Response
    {
        public Dictionary<string, object?> Account { get; set; }

        public Response()
        {
            Account = new Dictionary<string, object?>();
        }
    }
}
