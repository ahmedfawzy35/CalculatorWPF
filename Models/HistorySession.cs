using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CalculatorWPF.Models
{
    [XmlRoot("HistorySession")]
    public class HistorySession
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<DataRecord> Records { get; set; } = new();
        public double Total { get; set; }

        [XmlIgnore]
        public int Count => Records?.Count ?? 0;

        [XmlIgnore]
        public string FormattedTotal => Total.ToString("#,##0.##");

        [XmlIgnore]
        public string FormattedDate => CreatedAt.ToString("yyyy-MM-dd HH:mm");

        [XmlIgnore]
        public string DisplayText => $"{Name} - عدد الصفوف: {Count} - الإجمالي: {FormattedTotal}";

        public HistorySession()
        {
        }

        public HistorySession(int id, string name, IEnumerable<DataRecord> records, double total, DateTime? createdAt = null)
        {
            Id = id;
            Name = name;
            Records = new List<DataRecord>(records);
            Total = total;
            CreatedAt = createdAt ?? DateTime.Now;
        }
    }
}
