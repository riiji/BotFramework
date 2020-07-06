using System.ComponentModel.DataAnnotations;

namespace BotFramework.Database.Models
{
    public class BotSettings
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}