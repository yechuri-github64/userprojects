using System.Collections.Generic;

namespace CreateaccountsdemoLambda.Models
{
    public class AccountInput
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class Request
    {
        public ActionType Action { get; set; }

        // For Get and Delete actions, Id should be provided
        public string? Id { get; set; }

        // For Create action multiple accounts can be provided here
        public List<AccountInput> Accounts { get; set; }

        public Request()
        {
            Accounts = new List<AccountInput>();
        }
    }
}
