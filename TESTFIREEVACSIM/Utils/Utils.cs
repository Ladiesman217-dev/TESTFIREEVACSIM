using System;

namespace TESTFIREEVACSIM.Utils
{
    public static class Utils
    {
        private static readonly Random random = new Random();

        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}