using System;

namespace NumberLambda.Services
{
    public static class Service
    {
        private const int DefaultMin = 5;
        private const int DefaultMax = 100;

        public static int GenerateRandomNumber(int min = DefaultMin, int max = DefaultMax)
        {
            if (min < DefaultMin) min = DefaultMin;
            if (max > DefaultMax) max = DefaultMax;
            if (min > max)
            {
                min = DefaultMin;
                max = DefaultMax;
            }

            // Random.Shared is available on .NET 6+ and is thread-safe
            return Random.Shared.Next(min, max + 1);
        }
    }
}