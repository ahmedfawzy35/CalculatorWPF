using System;
using System.IO;
using System.Windows;
using CalculatorWPF.Services;
using CalculatorWPF.Views;

namespace CalculatorWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = AppSettingsStorage.LoadSettings();
            ApplyTheme(settings.SelectedTheme);

            var mainView = new MainView();
            mainView.Show();
        }

        public static void ApplyTheme(string themeName)
        {
            try
            {
                string fileName = themeName switch
                {
                    "Light" => "LightTheme",
                    "Orange" => "OrangeTheme",
                    "Red" => "RedTheme",
                    "Brown" => "BrownTheme",
                    _ => "DarkTheme"
                };

                var dict = new ResourceDictionary
                {
                    Source = new Uri($"Themes/{fileName}.xaml", UriKind.Relative)
                };

                Current.Resources.MergedDictionaries.Clear();
                Current.Resources.MergedDictionaries.Add(dict);
            }
            catch
            {
                // Ignore failure if window is shutting down
            }
        }

        public static void ApplyTheme(bool isDark)
        {
            ApplyTheme(isDark ? "Dark" : "Light");
        }
    }
}
