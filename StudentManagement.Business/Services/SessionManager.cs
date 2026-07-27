using StudentManagement.Domain.Entities;

namespace StudentManagement.Business.Services
{
    public sealed class SessionManager
    {
        private static readonly SessionManager _instance = new SessionManager();

        public static SessionManager Instance => _instance;

        private SessionManager() { }

        public User? CurrentUser { get; set; }

        public void Login(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool IsLoggedIn => CurrentUser != null;
    }
}
