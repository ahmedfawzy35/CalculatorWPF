using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;

namespace CalculatorWPF.ViewModels
{
    public partial class StandardCalculatorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _displayText = "0";

        [ObservableProperty]
        private string _expressionText = string.Empty;

        [ObservableProperty]
        private string _memoryText = string.Empty;

        [ObservableProperty]
        private bool _isMemoryActive;

        private double? _storedValue;
        private string? _pendingOperator;
        private bool _isNewInput = true;
        private double _memoryValue = 0;

        [RelayCommand]
        private void InputDigit(string digit)
        {
            if (digit == ".")
            {
                if (_isNewInput)
                {
                    DisplayText = "0.";
                    _isNewInput = false;
                }
                else if (!DisplayText.Contains("."))
                {
                    DisplayText += ".";
                }
                return;
            }

            if (_isNewInput || DisplayText == "0")
            {
                DisplayText = digit;
                _isNewInput = false;
            }
            else
            {
                if (DisplayText.Length < 16) // Max digits limit
                {
                    DisplayText += digit;
                }
            }
        }

        [RelayCommand]
        private void SetOperator(string op)
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentValue))
            {
                if (_storedValue.HasValue && !_isNewInput && _pendingOperator != null)
                {
                    CalculateResult();
                    if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double res))
                    {
                        _storedValue = res;
                    }
                }
                else
                {
                    _storedValue = currentValue;
                }

                _pendingOperator = op;
                ExpressionText = $"{FormatNumber(_storedValue.Value)} {op}";
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void CalculateEquals()
        {
            if (_storedValue.HasValue && _pendingOperator != null)
            {
                if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentValue))
                {
                    ExpressionText = $"{FormatNumber(_storedValue.Value)} {_pendingOperator} {FormatNumber(currentValue)} =";
                    CalculateResult();
                    _storedValue = null;
                    _pendingOperator = null;
                    _isNewInput = true;
                }
            }
        }

        private void CalculateResult()
        {
            if (!_storedValue.HasValue || _pendingOperator == null) return;

            if (!double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double secondVal)) return;

            double result = 0;
            switch (_pendingOperator)
            {
                case "+":
                    result = _storedValue.Value + secondVal;
                    break;
                case "-":
                case "−":
                    result = _storedValue.Value - secondVal;
                    break;
                case "×":
                case "*":
                    result = _storedValue.Value * secondVal;
                    break;
                case "÷":
                case "/":
                    if (Math.Abs(secondVal) < 1e-12)
                    {
                        DisplayText = "لا يمكن القسمة على صفر";
                        _storedValue = null;
                        _pendingOperator = null;
                        _isNewInput = true;
                        return;
                    }
                    result = _storedValue.Value / secondVal;
                    break;
            }

            DisplayText = FormatNumber(result);
        }

        [RelayCommand]
        private void ClearAll()
        {
            DisplayText = "0";
            ExpressionText = string.Empty;
            _storedValue = null;
            _pendingOperator = null;
            _isNewInput = true;
        }

        [RelayCommand]
        private void ClearEntry()
        {
            DisplayText = "0";
            _isNewInput = true;
        }

        [RelayCommand]
        private void Backspace()
        {
            if (_isNewInput || DisplayText == "0" || DisplayText.Length == 0) return;

            if (DisplayText.Length == 1 || (DisplayText.Length == 2 && DisplayText.StartsWith("-")))
            {
                DisplayText = "0";
                _isNewInput = true;
            }
            else
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            }
        }

        [RelayCommand]
        private void ToggleNegate()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                if (val != 0)
                {
                    val = -val;
                    DisplayText = FormatNumber(val);
                }
            }
        }

        [RelayCommand]
        private void CalculateSquareRoot()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                if (val < 0)
                {
                    DisplayText = "إدخال غير صالح";
                    _isNewInput = true;
                    return;
                }
                ExpressionText = $"√({FormatNumber(val)})";
                double res = Math.Sqrt(val);
                DisplayText = FormatNumber(res);
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void CalculateSquare()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                ExpressionText = $"sqr({FormatNumber(val)})";
                double res = val * val;
                DisplayText = FormatNumber(res);
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void CalculateReciprocal()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                if (Math.Abs(val) < 1e-12)
                {
                    DisplayText = "لا يمكن القسمة على صفر";
                    _isNewInput = true;
                    return;
                }
                ExpressionText = $"1/({FormatNumber(val)})";
                double res = 1.0 / val;
                DisplayText = FormatNumber(res);
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void CalculatePercent()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                double baseVal = _storedValue ?? 0;
                double res = baseVal != 0 ? (baseVal * val) / 100.0 : val / 100.0;
                DisplayText = FormatNumber(res);
                _isNewInput = true;
            }
        }

        // Memory Commands
        [RelayCommand]
        private void MemoryClear()
        {
            _memoryValue = 0;
            IsMemoryActive = false;
            MemoryText = string.Empty;
        }

        [RelayCommand]
        private void MemoryRecall()
        {
            if (IsMemoryActive)
            {
                DisplayText = FormatNumber(_memoryValue);
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void MemoryAdd()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                _memoryValue += val;
                IsMemoryActive = true;
                MemoryText = $"M: {FormatNumber(_memoryValue)}";
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void MemorySubtract()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                _memoryValue -= val;
                IsMemoryActive = true;
                MemoryText = $"M: {FormatNumber(_memoryValue)}";
                _isNewInput = true;
            }
        }

        [RelayCommand]
        private void MemoryStore()
        {
            if (double.TryParse(DisplayText, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                _memoryValue = val;
                IsMemoryActive = true;
                MemoryText = $"M: {FormatNumber(_memoryValue)}";
                _isNewInput = true;
            }
        }

        private string FormatNumber(double val)
        {
            if (double.IsNaN(val) || double.IsInfinity(val))
                return "خطأ";

            // If integer, format without decimal places
            if (Math.Abs(val % 1) < 1e-9)
            {
                return val.ToString("G15", CultureInfo.InvariantCulture);
            }
            return val.ToString("0.##############", CultureInfo.InvariantCulture);
        }
    }
}
