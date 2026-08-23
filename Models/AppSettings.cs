using CommunityToolkit.Mvvm.ComponentModel;

namespace CalculatorWPF.Models
{
    public partial class AppSettings : ObservableObject
    {
        [ObservableProperty]
        private string _selectedTheme = "Dark"; // Dark, Light, Orange, Red, Brown

        [ObservableProperty]
        private bool _enableAutoSave = true;

        [ObservableProperty]
        private int _autoDeleteDays = 0; // 0 = Never, 1, 2, 3, 7, 14, 30, 90
    }
}
