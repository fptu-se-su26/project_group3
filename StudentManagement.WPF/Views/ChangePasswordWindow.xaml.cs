using StudentManagement.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement.WPF.Views
{
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void txtCurrent_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm) vm.CurrentPassword = ((PasswordBox)sender).Password;
        }

        private void txtNew_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm) vm.NewPassword = ((PasswordBox)sender).Password;
        }

        private void txtConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChangePasswordViewModel vm) vm.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }
}
