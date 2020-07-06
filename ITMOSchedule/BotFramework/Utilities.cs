using System;
using System.Collections.Generic;
using System.Linq;
using BotFramework.BotFramework.Exceptions;
using BotFramework.Common;

namespace BotFramework.BotFramework
{
    public static class Utilities
    {
        private static readonly Random Random = new Random();

        public static int GetRandom()
        {
            return Random.Next();
        }
    }

}