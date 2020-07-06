using System;

namespace Tef.BotFramework.BotFramework.Exceptions
{
    public class BotValidException : ArgumentNullException 
    {
        public BotValidException()
        {

        }

        public BotValidException(string message) : base(message)
        {

        }
    }
}