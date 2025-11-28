using System.Text.Json.Serialization;

namespace TestfromdocspostgresqlLambda.Models
{
    public class Response
    {
        public string railcardId { get; set; } = null!;
        public string token { get; set; } = null!;
    }
}
