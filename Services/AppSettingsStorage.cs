using CalculatorWPF.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CalculatorWPF.Services
{
    public static class AppSettingsStorage
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(AppSettings));

        public static AppSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new AppSettings();
                }

                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var settings = Serializer.Deserialize(stream) as AppSettings;
                    return settings ?? new AppSettings();
                }
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                using (var writer = new StreamWriter(FilePath))
                {
                    Serializer.Serialize(writer, settings);
                }
            }
            catch
            {
                // Ignore write errors gracefully
            }
        }
    }
}
