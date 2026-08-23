using CalculatorWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CalculatorWPF.Services
{
    public static class XmlHistoryStorage
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.xml");
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(List<HistorySession>));

        public static List<HistorySession> LoadHistory()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new List<HistorySession>();
                }

                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var result = Serializer.Deserialize(stream) as List<HistorySession>;
                    return result ?? new List<HistorySession>();
                }
            }
            catch
            {
                return new List<HistorySession>();
            }
        }

        public static void SaveHistory(IEnumerable<HistorySession> sessions)
        {
            try
            {
                var list = new List<HistorySession>(sessions);
                using (var writer = new StreamWriter(FilePath))
                {
                    Serializer.Serialize(writer, list);
                }
            }
            catch
            {
                // Ignore write errors gracefully
            }
        }
    }
}
