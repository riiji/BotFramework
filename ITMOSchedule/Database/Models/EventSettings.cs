using System;

namespace BotFramework.Database.Models
{
    public class EventSettings
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime DateTime { get; set; }
    }
}