namespace RandomNumberGenerator.Models
{
 // Note: primary entry for this function is GET query parameters. This model is provided for documentation/sample usage.
 public class RandomRequest
 {
 // number of values to generate
 public int Count { get; set; } = 10;
 public int Min { get; set; } = 0;
 public int Max { get; set; } = 100;
 // optional seed for deterministic output
 public int? Seed { get; set; }
 // when true, generated numbers will be unique if possible
 public bool Unique { get; set; } = false;
 }
}
