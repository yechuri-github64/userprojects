namespace NumberLambda.Models
{
    // The request model is intentionally simple. Clients may optionally send an Action or other metadata.
    public class Request
    {
        public string? Action { get; set; }
    }
}
