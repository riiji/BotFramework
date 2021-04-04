using System;
using Kysect.BotFramework.Database;

namespace Kysect.BotFramework.Settings
{
    public class SqliteSettingsProvider<TSettings> : ISettingsProvider<TSettings> where TSettings : new()
    {
        private readonly string _connectionString;
        
        public SqliteSettingsProvider(string connectionString)
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