using System;
using Tef.BotFramework.Database;

namespace Tef.BotFramework.Settings
{
    public class SettingsFromDatabase<TSettings> : IGetSettings<TSettings> where TSettings : new()
    {
        private readonly string _connectionString;
        private readonly DatabaseType _databaseType;

        public SettingsFromDatabase(string connectionString, DatabaseType databaseType)
        {
            _connectionString = connectionString;
            _databaseType = databaseType;
        }

        public TSettings GetSettings()
        {
            var settings = new TSettings();
            using var context = new DatabaseContext(_connectionString, _databaseType);

            foreach (var property in typeof(TSettings).GetProperties())
            {
                var value = context.BotSettings.Find(property.Name).Value;
                property.SetValue(settings, Convert.ChangeType(value, property.PropertyType));
            }

            return settings;
        }
    }
}