using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Xml.Serialization;

namespace CalculatorWPF.Models
{
    [XmlRoot("DataRecord")]
    public partial class DataRecord : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private double _numberOne;

        [ObservableProperty]
        private double _numberTwo;

        [ObservableProperty]
        private double _result;

        [XmlIgnore]
        public string FormattedId => $"#{Id:D4}";

        public DataRecord()
        {
        }

        public DataRecord(int id, double numberOne, double numberTwo)
        {
            Id = id;
            NumberOne = numberOne;
            NumberTwo = numberTwo;
            RecalculateResult();
        }

        public void RecalculateResult()
        {
            Result = Math.Round(NumberOne * NumberTwo, 2);
        }
    }
}
