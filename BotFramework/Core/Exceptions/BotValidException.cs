using System;

namespace Tef.BotFramework.Core.Exceptions
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