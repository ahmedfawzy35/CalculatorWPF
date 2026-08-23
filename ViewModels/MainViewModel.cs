using CalculatorWPF.Models;
using CalculatorWPF.Services;
using CalculatorWPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace CalculatorWPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<DataRecord> _records = new();

        [ObservableProperty]
        private ObservableCollection<HistorySession> _historySessions = new();

        [ObservableProperty]
        private ObservableCollection<HistorySession> _todayHistorySessions = new();

        [ObservableProperty]
        private ObservableCollection<HistorySession> _filteredHistorySessions = new();

        [ObservableProperty]
        private int? _currentSessionId;

        [ObservableProperty]
        private string _currentSessionName = string.Empty;

        [ObservableProperty]
        private int _selectedTabIndex;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _selectedDateFilter = "All"; // All, Today, Week, Month

        [ObservableProperty]
        private string _currentInput = string.Empty;

        [ObservableProperty]
        private double? _numberOne;

        [ObservableProperty]
        private bool _isEnteringSecondNumber;

        [ObservableProperty]
        private string _equationText = "العملية الحالية";

        [ObservableProperty]
        private string _instantResultText = "0";

        [ObservableProperty]
        private double _total;

        [ObservableProperty]
        private int _count;

        [ObservableProperty]
        private DataRecord? _selectedRecord;

        [ObservableProperty]
        private int _selectedSettingsSubTab;

        [ObservableProperty]
        private AppSettings _settings = new();

        [ObservableProperty]
        private bool _isDarkTheme = true;

        [ObservableProperty]
        private string _themeToggleText = "المظهر الداكن 🌙";

        public Func<string, string, bool>? ShowWarningDialog { get; set; }

        public MainViewModel()
        {
            Settings = AppSettingsStorage.LoadSettings();
            App.ApplyTheme(Settings.SelectedTheme);
            LoadHistoryFromStorage();
            ApplyAutoCleanup();
            RecalculateTotals();
        }

        private void LoadHistoryFromStorage()
        {
            var loaded = XmlHistoryStorage.LoadHistory();
            HistorySessions.Clear();
            foreach (var session in loaded.OrderBy(s => s.Id))
            {
                HistorySessions.Add(session);
            }
            ApplyFilters();
        }

        partial void OnSearchQueryChanged(string value)
        {
            ApplyFilters();
        }

        partial void OnSelectedDateFilterChanged(string value)
        {
            ApplyFilters();
        }

        [RelayCommand]
        private void SetDateFilter(string filter)
        {
            SelectedDateFilter = filter;
        }

        public void ApplyFilters()
        {
            var query = HistorySessions.AsEnumerable();

            // Date filtering
            switch (SelectedDateFilter)
            {
                case "Today":
                    query = query.Where(s => s.CreatedAt.Date == DateTime.Today);
                    break;
                case "Week":
                    var weekAgo = DateTime.Today.AddDays(-7);
                    query = query.Where(s => s.CreatedAt.Date >= weekAgo);
                    break;
                case "Month":
                    var monthAgo = DateTime.Today.AddDays(-30);
                    query = query.Where(s => s.CreatedAt.Date >= monthAgo);
                    break;
                case "All":
                default:
                    break;
            }

            // Text Search filtering
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                string search = SearchQuery.Trim().ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(search) ||
                    s.FormattedDate.Contains(search) ||
                    s.FormattedTotal.Contains(search) ||
                    s.Total.ToString().Contains(search) ||
                    s.Id.ToString().Contains(search)
                );
            }

            FilteredHistorySessions.Clear();
            foreach (var item in query.OrderBy(s => s.Id))
            {
                FilteredHistorySessions.Add(item);
            }

            TodayHistorySessions.Clear();
            foreach (var item in HistorySessions.Where(s => s.CreatedAt.Date == DateTime.Today).OrderBy(s => s.Id))
            {
                TodayHistorySessions.Add(item);
            }
        }

        [RelayCommand]
        private void ProcessInput()
        {
            string trimmed = CurrentInput.Trim();

            double val = 1;
            if (string.IsNullOrEmpty(trimmed))
            {
                val = 1;
            }
            else
            {
                if (!double.TryParse(trimmed, out val))
                {
                    ShowWarningDialog?.Invoke("تنبيه الإدخال", "يرجى كتابة رقم صحيح أو عشري!");
                    CurrentInput = string.Empty;
                    return;
                }
            }

            if (Math.Abs(val) < 0.000001)
            {
                ShowWarningDialog?.Invoke("تنبيه الإدخال", "يُمنع إدخال الرقم صفر!");
                CurrentInput = string.Empty;
                return;
            }

            if (!IsEnteringSecondNumber)
            {
                // First Number entered
                NumberOne = val;
                IsEnteringSecondNumber = true;
                EquationText = $"{FormatNumber(NumberOne.Value)} × ...";
                InstantResultText = FormatNumber(NumberOne.Value);
                CurrentInput = string.Empty;
            }
            else
            {
                // Second Number entered -> execute multiplication
                double num1 = NumberOne ?? 1;
                double num2 = val;
                double result = Math.Round(num1 * num2, 2);

                int nextId = Records.Count > 0 ? Records.Max(r => r.Id) + 1 : 1;
                var record = new DataRecord(nextId, num1, num2);
                Records.Add(record);

                EquationText = $"{FormatNumber(num1)} × {FormatNumber(num2)}";
                InstantResultText = FormatNumber(result);

                RecalculateTotals();

                // Reset state for next sequence
                NumberOne = null;
                IsEnteringSecondNumber = false;
                CurrentInput = string.Empty;
            }
        }

        [RelayCommand]
        private void ArchiveSession()
        {
            if (Records.Count == 0) return;

            string sessionName = CurrentSessionName.Trim();
            int nextId = CurrentSessionId ?? (HistorySessions.Count > 0 ? HistorySessions.Max(s => s.Id) + 1 : 1);

            if (string.IsNullOrWhiteSpace(sessionName))
            {
                sessionName = $"العملية {nextId}";
            }

            if (CurrentSessionId.HasValue)
            {
                // Modifying original session in-place! (No duplicate saved)
                var existing = HistorySessions.FirstOrDefault(s => s.Id == CurrentSessionId.Value);
                if (existing != null)
                {
                    existing.Name = sessionName;
                    existing.Records = new List<DataRecord>(Records);
                    existing.Total = Total;
                }
                else
                {
                    var newSession = new HistorySession(nextId, sessionName, Records, Total);
                    HistorySessions.Add(newSession);
                }
            }
            else
            {
                // Creating new session
                var newSession = new HistorySession(nextId, sessionName, Records, Total);
                HistorySessions.Add(newSession);
            }

            // Re-sort history by ID
            var sortedList = HistorySessions.OrderBy(s => s.Id).ToList();
            HistorySessions.Clear();
            foreach (var item in sortedList)
            {
                HistorySessions.Add(item);
            }

            // Persist to XML if auto-save is enabled
            if (Settings.EnableAutoSave)
            {
                XmlHistoryStorage.SaveHistory(HistorySessions);
            }
            ApplyFilters();

            // Clear active workspace
            Records.Clear();
            CurrentSessionId = null;
            CurrentSessionName = string.Empty;
            NumberOne = null;
            IsEnteringSecondNumber = false;
            CurrentInput = string.Empty;
            EquationText = "العملية الحالية";
            InstantResultText = "0";

            RecalculateTotals();
        }

        [RelayCommand]
        private void LoadSession(HistorySession session)
        {
            if (session == null) return;

            // Auto-archive unsaved work if currently on new session with records
            if (Records.Count > 0 && !CurrentSessionId.HasValue)
            {
                ArchiveSession();
            }

            CurrentSessionId = session.Id;
            CurrentSessionName = session.Name;

            Records.Clear();
            foreach (var r in session.Records)
            {
                Records.Add(new DataRecord(r.Id, r.NumberOne, r.NumberTwo));
            }

            NumberOne = null;
            IsEnteringSecondNumber = false;
            CurrentInput = string.Empty;

            if (Records.Count > 0)
            {
                var last = Records.Last();
                EquationText = $"{FormatNumber(last.NumberOne)} × {FormatNumber(last.NumberTwo)}";
                InstantResultText = FormatNumber(last.Result);
            }
            else
            {
                EquationText = "العملية الحالية";
                InstantResultText = "0";
            }

            RecalculateTotals();

            // Switch to Calculator tab
            SelectedTabIndex = 0;
        }

        [RelayCommand]
        private void DeleteSession(HistorySession? session)
        {
            if (session == null) return;

            if (HistorySessions.Contains(session))
            {
                HistorySessions.Remove(session);
                XmlHistoryStorage.SaveHistory(HistorySessions);
                ApplyFilters();

                if (CurrentSessionId == session.Id)
                {
                    CurrentSessionId = null;
                    CurrentSessionName = string.Empty;
                    Records.Clear();
                    RecalculateTotals();
                }
            }
        }

        [RelayCommand]
        private void NewSession()
        {
            if (Records.Count > 0)
            {
                ArchiveSession();
            }

            CurrentSessionId = null;
            CurrentSessionName = string.Empty;
            Records.Clear();
            NumberOne = null;
            IsEnteringSecondNumber = false;
            CurrentInput = string.Empty;
            EquationText = "العملية الحالية";
            InstantResultText = "0";

            RecalculateTotals();
            SelectedTabIndex = 0;
        }

        [RelayCommand]
        private void DeleteRecord(DataRecord? record)
        {
            var target = record ?? SelectedRecord;
            if (target != null && Records.Contains(target))
            {
                Records.Remove(target);
                RecalculateTotals();

                // If editing an active session, auto-update XML
                if (CurrentSessionId.HasValue)
                {
                    var existing = HistorySessions.FirstOrDefault(s => s.Id == CurrentSessionId.Value);
                    if (existing != null)
                    {
                        existing.Records = new List<DataRecord>(Records);
                        existing.Total = Total;
                        XmlHistoryStorage.SaveHistory(HistorySessions);
                        ApplyFilters();
                    }
                }
            }
        }

        [RelayCommand]
        private void EditRecord(DataRecord? record)
        {
            var target = record ?? SelectedRecord;
            if (target == null) return;

            var editVm = new EditRowViewModel(target);
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) 
                               ?? Application.Current.MainWindow;

            var editView = new EditRowView
            {
                DataContext = editVm,
                Owner = activeWindow
            };

            editVm.CloseAction = (saved) =>
            {
                editView.DialogResult = saved;
                editView.Close();
            };

            if (editView.ShowDialog() == true)
            {
                RecalculateTotals();

                // Auto-sync active session if saved
                if (CurrentSessionId.HasValue)
                {
                    var existing = HistorySessions.FirstOrDefault(s => s.Id == CurrentSessionId.Value);
                    if (existing != null)
                    {
                        existing.Records = new List<DataRecord>(Records);
                        existing.Total = Total;
                        XmlHistoryStorage.SaveHistory(HistorySessions);
                        ApplyFilters();
                    }
                }
            }
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            IsDarkTheme = !IsDarkTheme;
            UpdateTheme();
        }

        public void SetTheme(bool isDark)
        {
            IsDarkTheme = isDark;
            UpdateTheme();
        }

        private void UpdateTheme()
        {
            ThemeToggleText = IsDarkTheme ? "المظهر الداكن 🌙" : "المظهر الفاتح ☀️";
            App.ApplyTheme(IsDarkTheme);
            SaveThemePreference(IsDarkTheme);
        }

        private void SaveThemePreference(bool isDark)
        {
            try
            {
                string themePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.txt");
                File.WriteAllText(themePath, isDark ? "dark" : "light");
            }
            catch
            {
                // Ignore IO errors
            }
        }

        public void RecalculateTotals()
        {
            Count = Records.Count;
            Total = Math.Round(Records.Sum(r => r.Result), 2);
        }

        [RelayCommand]
        private void SelectTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName)) return;
            Settings.SelectedTheme = themeName;
            AppSettingsStorage.SaveSettings(Settings);
            App.ApplyTheme(themeName);
        }

        [RelayCommand]
        private void SetAutoDeleteDays(object parameter)
        {
            if (parameter != null && int.TryParse(parameter.ToString(), out int days))
            {
                Settings.AutoDeleteDays = days;
                AppSettingsStorage.SaveSettings(Settings);
                ApplyAutoCleanup();
            }
        }

        [RelayCommand]
        private void ToggleAutoSave()
        {
            Settings.EnableAutoSave = !Settings.EnableAutoSave;
            AppSettingsStorage.SaveSettings(Settings);
        }

        public void SaveSettings()
        {
            AppSettingsStorage.SaveSettings(Settings);
            ApplyAutoCleanup();
        }

        public void ApplyAutoCleanup()
        {
            if (Settings.AutoDeleteDays <= 0 || HistorySessions.Count == 0) return;

            var cutoffDate = DateTime.Today.AddDays(-Settings.AutoDeleteDays);
            var toRemove = HistorySessions.Where(s => s.CreatedAt.Date < cutoffDate).ToList();

            if (toRemove.Count > 0)
            {
                foreach (var item in toRemove)
                {
                    HistorySessions.Remove(item);
                }
                if (Settings.EnableAutoSave)
                {
                    XmlHistoryStorage.SaveHistory(HistorySessions);
                }
                ApplyFilters();
            }
        }

        [RelayCommand]
        private void ClearAllData()
        {
            bool confirmed = ShowWarningDialog?.Invoke("تفريغ كافة البيانات", "هل أنت تأكد من مسح جميع البيانات والعمليات المؤرشفة نهائياً؟ هذا الإجراء لا يمكن التراجع عنه!") ?? true;

            if (confirmed)
            {
                Records.Clear();
                HistorySessions.Clear();
                TodayHistorySessions.Clear();
                FilteredHistorySessions.Clear();
                CurrentSessionId = null;
                CurrentSessionName = string.Empty;
                NumberOne = null;
                IsEnteringSecondNumber = false;
                CurrentInput = string.Empty;
                EquationText = "العملية الحالية";
                InstantResultText = "0";

                XmlHistoryStorage.SaveHistory(HistorySessions);
                RecalculateTotals();
            }
        }

        private static string FormatNumber(double val)
        {
            return val.ToString("#,##0.##");
        }
    }
}
