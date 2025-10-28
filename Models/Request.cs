using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TestsflambdaLambda.Models
{
    public class Request
    {
        public Request()
        {
            Operation = string.Empty;
            Accounts = new List<AccountDto>();
            Id = string.Empty;
        }

        [JsonPropertyName("operation")]
        public string Operation { get; set; }

        // For create: multiple accounts can be provided.
        // For update: only the first account in the array will be processed.
        [JsonPropertyName("accounts")]
        public List<AccountDto> Accounts { get; set; }

        // For retrieve/delete one id at a time
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class AccountDto
    {
        public AccountDto()
        {
            Id = string.Empty;
            Name = string.Empty;
            Phone = string.Empty;
            Website = string.Empty;
            // Add other fields as needed
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("website")]
        public string Website { get; set; }

        // Add other Account fields if required
    }
}
