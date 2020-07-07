using System;
using Tef.BotFramework.Database;

namespace Tef.BotFramework.Settings
{
    public class SettingsFromSqlite<TSettings> : IGetSettings<TSettings> where TSettings : new()
    {
        private readonly string _connectionString;
        
        public SettingsFromSqlite(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TSettings GetSettings()
        {
            var settings = new TSettings();
            using var context = new DatabaseContext(_connectionString);

            foreach (var property in typeof(TSettings).GetProperties())
            {
                var value = context.BotSettings.Find(property.Name).Value;
                property.SetValue(settings, Convert.ChangeType(value, property.PropertyType));
            }

            return settings;
        }
    }
}