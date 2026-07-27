using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Services;
using StudentManagement.WPF.Commands;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public ICommand SaveCommand { get; }

        public delegate void CloseActionHandler();
        public event CloseActionHandler? OnRequestClose;

        public ChangePasswordViewModel(IUserService userService)
        {
            _userService = userService;
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
        }

        private async Task SaveAsync()
        {
            var userId = SessionManager.Instance.CurrentUser?.UserId;
            if (userId == null) return;

            var (success, message) = await _userService.ChangePasswordAsync(userId.Value, CurrentPassword, NewPassword, ConfirmPassword);
            MessageBox.Show(message);
            if (success) OnRequestClose?.Invoke();
        }
    }
}
