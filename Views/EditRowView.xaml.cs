using CalculatorWPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CalculatorWPF.Views
{
    public partial class EditRowView : Window
    {
        public EditRowView()
        {
            InitializeComponent();
            Loaded += (s, e) => FirstNumberTextBox.Focus();
        }

        private void FirstNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                SecondNumberTextBox.Focus();
                SecondNumberTextBox.SelectAll();
            }
        }

        private void SecondNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (DataContext is EditRowViewModel vm)
                {
                    vm.SaveCommand.Execute(null);
                }
            }
        }
    }
}
