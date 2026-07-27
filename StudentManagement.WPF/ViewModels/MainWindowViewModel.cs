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

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotDashboard));
            }
        }

        public bool IsNotDashboard => CurrentView != null && !(CurrentView is DashboardView);

        public ICommand LogoutCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand GoToDashboardCommand { get; }

        public event Action? OnLogoutRequested;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // Initial view is Dashboard
            GoToDashboard();

            LogoutCommand = new RelayCommand(_ => Logout());
            ChangePasswordCommand = new RelayCommand(_ => OpenChangePassword());
            GoToDashboardCommand = new RelayCommand(_ => GoToDashboard());

            BuildMenu();
        }

        private void BuildMenu()
        {
            var role = SessionManager.Instance.CurrentUser?.Role.RoleName ?? string.Empty;

            if (role == "Administrator")
            {
                MenuItems.Add(new MenuItemViewModel("User & Role Management", new RelayCommand(_ => OpenView<UserManagementView>())));
            }

            if (role is "Administrator" or "AcademicStaff")
            {
                MenuItems.Add(new MenuItemViewModel("Student Management", new RelayCommand(_ => OpenView<StudentManagementView>())));
                MenuItems.Add(new MenuItemViewModel("Academic Management", new RelayCommand(_ => OpenView<AcademicManagementView>())));
                MenuItems.Add(new MenuItemViewModel("Course & Subject Management", new RelayCommand(_ => OpenView<CourseManagementView>())));
            }

            if (role == "Student")
            {
                MenuItems.Add(new MenuItemViewModel("Course Registration", new RelayCommand(_ => OpenView<CourseRegistrationView>())));
            }

            if (role is "Administrator" or "Lecturer")
            {
                MenuItems.Add(new MenuItemViewModel("Grade Management", new RelayCommand(_ => OpenView<GradeManagementView>())));
            }

            if (role is "Administrator" or "Accountant")
            {
                MenuItems.Add(new MenuItemViewModel("Finance & Tuition", new RelayCommand(_ => OpenView<FinanceReportingView>())));
            }

            if (role is "Administrator" or "Accountant" or "AcademicStaff" or "Student")
            {
                MenuItems.Add(new MenuItemViewModel("Reports & Results", new RelayCommand(_ => OpenView<ReportsView>())));
            }
        }

        private void OpenView<TControl>() where TControl : System.Windows.Controls.UserControl
        {
            var view = _serviceProvider.GetRequiredService<TControl>();
            CurrentView = view;
        }

        private void GoToDashboard()
        {
            var dashboard = _serviceProvider.GetRequiredService<DashboardView>();
            CurrentView = dashboard;
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
            OnLogoutRequested?.Invoke();
        }
    }
}
