using System.Windows;
using System.Windows.Controls;
using StudentManagement.WPF.ViewModels;

namespace StudentManagement.WPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
