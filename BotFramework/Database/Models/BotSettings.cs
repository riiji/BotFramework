using System.ComponentModel.DataAnnotations;

namespace Tef.BotFramework.Database.Models
{
    public class BotSettings
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}