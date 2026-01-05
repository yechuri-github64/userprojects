using System;
using System.Text.Json.Serialization;

#nullable enable

namespace DemotestprojLambda.Models
{
    public class Response
    {
        [JsonPropertyName("travelcardId")]
        public string travelcardId { get; set; } = null!;

        [JsonPropertyName("token")]
        public string token { get; set; } = null!;
    }
}
