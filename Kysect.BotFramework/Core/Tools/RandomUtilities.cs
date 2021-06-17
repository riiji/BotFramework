using System;

namespace Kysect.BotFramework.Core.Tools
{
    public static class RandomUtilities
    {
        private static readonly Random Random = new Random();

        public static int GetRandom()
        {
            return Random.Next();
        }
    }

}