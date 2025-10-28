using System.Collections.Generic;

namespace TestSfLambdaLambda.Models
{
    public class Request
    {
        // operation: create/get/update/delete
        public string? Operation { get; set; }

        // For create: multiple accounts
        public List<Account>? Accounts { get; set; }

        // For get/update/delete: single Account Id
        public string? AccountId { get; set; }

        // For update: the account payload
        public Account? Account { get; set; }

        public Request()
        {
            Operation = null;
            Accounts = new List<Account>();
            AccountId = null;
            Account = null;
        }
    }

    public class Account
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Industry { get; set; }

        public Account()
        {
            Id = null;
            Name = null;
            Phone = null;
            Industry = null;
        }
    }
}
