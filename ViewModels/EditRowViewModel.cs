using CalculatorWPF.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace CalculatorWPF.ViewModels
{
    public partial class EditRowViewModel : ObservableObject
    {
        private readonly DataRecord _record;

        [ObservableProperty]
        private string _numberOneText;

        [ObservableProperty]
        private string _numberTwoText;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public Action<bool>? CloseAction { get; set; }

        public EditRowViewModel(DataRecord record)
        {
            _record = record;
            _numberOneText = record.NumberOne.ToString();
            _numberTwoText = record.NumberTwo.ToString();
        }

        [RelayCommand]
        private void Save()
        {
            ErrorMessage = string.Empty;

            double num1 = 1;
            double num2 = 1;

            if (string.IsNullOrWhiteSpace(NumberOneText))
            {
                num1 = 1;
            }
            else if (!double.TryParse(NumberOneText, out num1))
            {
                ErrorMessage = "الرقم الأول غير صحيح!";
                return;
            }

            if (string.IsNullOrWhiteSpace(NumberTwoText))
            {
                num2 = 1;
            }
            else if (!double.TryParse(NumberTwoText, out num2))
            {
                ErrorMessage = "الرقم الثاني غير صحيح!";
                return;
            }

            if (Math.Abs(num1) < 0.000001 || Math.Abs(num2) < 0.000001)
            {
                ErrorMessage = "يُمنع إدخال الرقم صفر!";
                return;
            }

            _record.NumberOne = num1;
            _record.NumberTwo = num2;
            _record.RecalculateResult();

            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
