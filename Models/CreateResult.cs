using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace accounts_sf_sa.Models
{
    public class CreateResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new();
    }
}
