using System;
using CalculatorWPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CalculatorWPF.Views
{
    public partial class MainView : Window
    {
        public MainViewModel ViewModel { get; }

        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;

            ViewModel.ShowWarningDialog = (title, message) =>
            {
                MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            };

            Loaded += (s, e) =>
            {
                AdjustWindowBoundsToWorkArea();
                FocusInput();
            };
        }

        private void AdjustWindowBoundsToWorkArea()
        {
            double workHeight = SystemParameters.WorkArea.Height;
            double workWidth = SystemParameters.WorkArea.Width;

            if (Height > workHeight - 20)
            {
                Height = Math.Max(MinHeight, workHeight - 40);
            }

            if (Width > workWidth - 20)
            {
                Width = Math.Max(MinWidth, workWidth - 40);
            }

            Top = Math.Max(0, (workHeight - Height) / 2);
            Left = Math.Max(0, (workWidth - Width) / 2);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (ViewModel.SelectedTabIndex == 1)
            {
                if (Win11CalculatorControl.HandleKeyDown(e.Key))
                {
                    e.Handled = true;
                }
            }
        }

        private void MainInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                ViewModel.ProcessInputCommand.Execute(null);
                FocusInput();
            }
        }

        public void FocusInput()
        {
            MainInputTextBox.Focus();
            MainInputTextBox.SelectAll();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
