using System;

namespace Kysect.BotFramework.Core.Exceptions
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