using System;

namespace NumberLambda.Models
{
    public class Request
    {
        // Optional overrides (will be clamped to 5-100)
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}