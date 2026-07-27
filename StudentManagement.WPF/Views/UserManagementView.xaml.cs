using StudentManagement.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace StudentManagement.WPF.Views
{
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();
        }

        private void txtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserManagementViewModel vm)
            {
                vm.NewPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}

