using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CreateaccountsdemoLambda.Models
{
    public class AccountModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class Request
    {
        // Operation: Get, Create, Update, Delete
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OperationType Operation { get; set; }

        // For Create: use Records (multiple)
        public List<AccountModel>? Records { get; set; }

        // For Get / Update / Delete: single record (Update uses Id and fields to update)
        public AccountModel? Record { get; set; }
    }
}
