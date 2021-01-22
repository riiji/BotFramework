using System;

namespace Tef.BotFramework.Core
{
    //TODO: move to tools?
    public static class Utilities
    {
        private static readonly Random Random = new Random();

        public static int GetRandom()
        {
            return Random.Next();
        }
    }

}