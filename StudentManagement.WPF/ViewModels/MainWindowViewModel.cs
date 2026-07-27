using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Business.Services;
using StudentManagement.WPF.Commands;
using StudentManagement.WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;

        public string CurrentUserDisplay =>
            $"{SessionManager.Instance.CurrentUser?.Username} ({SessionManager.Instance.CurrentUser?.Role.RoleName})";

        public ObservableCollection<MenuItemViewModel> MenuItems { get; } = new();

        public DashboardViewModel Dashboard { get; }

        public ICommand LogoutCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public event Action? OnLogout;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Dashboard = _serviceProvider.GetRequiredService<DashboardViewModel>();

            LogoutCommand = new RelayCommand(_ => Logout());
            ChangePasswordCommand = new RelayCommand(_ => OpenChangePassword());

            BuildMenu();
        }

        private void BuildMenu()
        {
            var role = SessionManager.Instance.CurrentUser?.Role.RoleName ?? string.Empty;

            if (role == "Administrator")
            {
                MenuItems.Add(new MenuItemViewModel("User & Role Management", new RelayCommand(_ => OpenWindow<UserManagementView>())));
            }

            if (role is "Administrator" or "AcademicStaff")
            {
                MenuItems.Add(new MenuItemViewModel("Student Management", new RelayCommand(_ => OpenWindow<StudentManagementView>())));
                MenuItems.Add(new MenuItemViewModel("Academic Management", new RelayCommand(_ => OpenWindow<AcademicManagementView>())));
                MenuItems.Add(new MenuItemViewModel("Course & Subject Management", new RelayCommand(_ => OpenWindow<CourseManagementView>())));
            }

            if (role == "Student")
            {
                MenuItems.Add(new MenuItemViewModel("Course Registration", new RelayCommand(_ => OpenWindow<CourseRegistrationView>())));
            }

            if (role is "Administrator" or "Lecturer")
            {
                MenuItems.Add(new MenuItemViewModel("Grade Management", new RelayCommand(_ => OpenWindow<GradeManagementView>())));
            }

            if (role is "Administrator" or "Accountant")
            {
                MenuItems.Add(new MenuItemViewModel("Finance & Tuition", new RelayCommand(_ => OpenWindow<FinanceReportingView>())));
            }

            if (role is "Administrator" or "Accountant" or "AcademicStaff" or "Student")
            {
                MenuItems.Add(new MenuItemViewModel("Reports & Results", new RelayCommand(_ => OpenWindow<ReportsView>())));
            }
        }

        private void OpenWindow<TWindow>() where TWindow : System.Windows.Window
        {
            var window = _serviceProvider.GetRequiredService<TWindow>();
            window.Show();
        }

        private void OpenChangePassword()
        {
            var window = _serviceProvider.GetRequiredService<ChangePasswordWindow>();
            if (window.DataContext is ChangePasswordViewModel vm)
            {
                vm.OnRequestClose += () => window.Close();
            }
            window.ShowDialog();
        }

        private void Logout()
        {
            SessionManager.Instance.Logout();
            OnLogout?.Invoke();
        }
    }
}
