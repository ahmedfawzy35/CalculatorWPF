using CalculatorWPF.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorWPF.Views
{
    public partial class StandardCalculatorControl : UserControl
    {
        public StandardCalculatorViewModel ViewModel { get; }

        public StandardCalculatorControl()
        {
            InitializeComponent();
            ViewModel = new StandardCalculatorViewModel();
            DataContext = ViewModel;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleKeyDown(e.Key))
            {
                e.Handled = true;
            }
        }

        public bool HandleKeyDown(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                int digit = key - Key.D0;
                ViewModel.InputDigitCommand.Execute(digit.ToString());
                return true;
            }
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                int digit = key - Key.NumPad0;
                ViewModel.InputDigitCommand.Execute(digit.ToString());
                return true;
            }
            if (key == Key.OemPeriod || key == Key.Decimal)
            {
                ViewModel.InputDigitCommand.Execute(".");
                return true;
            }
            if (key == Key.Add)
            {
                ViewModel.SetOperatorCommand.Execute("+");
                return true;
            }
            if (key == Key.Subtract || key == Key.OemMinus)
            {
                ViewModel.SetOperatorCommand.Execute("-");
                return true;
            }
            if (key == Key.Multiply)
            {
                ViewModel.SetOperatorCommand.Execute("×");
                return true;
            }
            if (key == Key.Divide || key == Key.OemQuestion)
            {
                ViewModel.SetOperatorCommand.Execute("÷");
                return true;
            }
            if (key == Key.Enter || key == Key.Return)
            {
                ViewModel.CalculateEqualsCommand.Execute(null);
                return true;
            }
            if (key == Key.Back)
            {
                ViewModel.BackspaceCommand.Execute(null);
                return true;
            }
            if (key == Key.Escape)
            {
                ViewModel.ClearAllCommand.Execute(null);
                return true;
            }

            return false;
        }
    }
}
