using System;

namespace NumberLambda.Services
{
    public class Service
    {
        private readonly Random _rng = new Random();

        /// <summary>
        /// Generate a random integer between 5 and 100 inclusive.
        /// </summary>
        public int GenerateRandomNumber()
        {
            // Random.Next(minValue, maxValueExclusive) -> use 101 to include 100
            return _rng.Next(5, 101);
        }
    }
}
