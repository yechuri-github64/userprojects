using System.Collections.Generic;

namespace RandomNumberGenerator.Models
{
 public class RandomResponse
 {
 public List<int> Numbers { get; set; } = new List<int>();
 public int Count => Numbers?.Count ?? 0;
 public Metadata Metadata { get; set; } = new Metadata();
 }

 public class Metadata
 {
 public int Min { get; set; }
 public int Max { get; set; }
 public bool Unique { get; set; }
 public int? Seed { get; set; }
 }
}
