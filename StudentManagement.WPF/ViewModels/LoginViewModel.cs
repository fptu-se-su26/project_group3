using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Services;
using StudentManagement.WPF.Commands;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand LoginCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;
        }

        private async Task LoginAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var (isSuccess, message, user) = await _authService.AuthenticateAsync(Username, Password);

            if (isSuccess && user != null)
            {
                SessionManager.Instance.Login(user);
                // Trigger event to close login window and open main window
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = message;
            }

            IsBusy = false;
        }

        public delegate void LoginSuccessHandler();
        public event LoginSuccessHandler? OnLoginSuccess;
    }
}
