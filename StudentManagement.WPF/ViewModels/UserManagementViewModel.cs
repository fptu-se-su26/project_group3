using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        private ObservableCollection<User> _users = new();
        public ObservableCollection<User> Users { get => _users; set { _users = value; OnPropertyChanged(); } }

        private ObservableCollection<Role> _roles = new();
        public ObservableCollection<Role> Roles { get => _roles; set { _roles = value; OnPropertyChanged(); } }

        public User? SelectedUser { get; set; }
        public Role? SelectedRoleForAssignment { get; set; }

        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

        private string _newUsername = string.Empty;
        public string NewUsername { get => _newUsername; set { _newUsername = value; OnPropertyChanged(); } }

        private string _newPassword = string.Empty;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }

        public Role? SelectedRoleForCreate { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand CreateUserCommand { get; }
        public ICommand AssignRoleCommand { get; }
        public ICommand LockCommand { get; }
        public ICommand UnlockCommand { get; }

        public UserManagementViewModel(IUserService userService)
        {
            _userService = userService;
            SearchCommand = new RelayCommand(async _ => await LoadUsersAsync());
            CreateUserCommand = new RelayCommand(async _ => await CreateUserAsync());
            AssignRoleCommand = new RelayCommand(async _ => await AssignRoleAsync(), _ => SelectedUser != null && SelectedRoleForAssignment != null);
            LockCommand = new RelayCommand(async _ => await SetLockAsync(false), _ => SelectedUser != null);
            UnlockCommand = new RelayCommand(async _ => await SetLockAsync(true), _ => SelectedUser != null);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var roles = await _userService.GetAllRolesAsync();
            Roles = new ObservableCollection<Role>(roles);
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var data = await _userService.GetAllUsersAsync(SearchText);
                Users = new ObservableCollection<User>(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}");
            }
        }

        private async Task CreateUserAsync()
        {
            if (SelectedRoleForCreate == null)
            {
                MessageBox.Show("Please select a role.");
                return;
            }

            var (success, message) = await _userService.CreateUserAsync(NewUsername, NewPassword, SelectedRoleForCreate.RoleId);
            MessageBox.Show(message);
            if (success)
            {
                NewUsername = string.Empty;
                NewPassword = string.Empty;
                await LoadUsersAsync();
            }
        }

        private async Task AssignRoleAsync()
        {
            if (SelectedUser == null || SelectedRoleForAssignment == null) return;
            var (success, message) = await _userService.AssignRoleAsync(SelectedUser.UserId, SelectedRoleForAssignment.RoleId);
            MessageBox.Show(message);
            if (success) await LoadUsersAsync();
        }

        private async Task SetLockAsync(bool isActive)
        {
            if (SelectedUser == null) return;
            var (success, message) = await _userService.SetLockStatusAsync(SelectedUser.UserId, isActive);
            MessageBox.Show(message);
            if (success) await LoadUsersAsync();
        }
    }
}
