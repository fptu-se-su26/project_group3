using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using StudentManagement.DataAccess;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Services;
using StudentManagement.WPF.Views;
using StudentManagement.WPF.ViewModels;

namespace StudentManagement.WPF
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            // Services
            services.AddTransient<IFinanceGradeService, FinanceGradeService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IAcademicService, AcademicService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IAuthService, AuthService>();

            // ViewModels
            services.AddTransient<GradeManagementViewModel>();
            services.AddTransient<FinanceReportingViewModel>();
            services.AddTransient<CourseManagementViewModel>();
            services.AddTransient<CourseRegistrationViewModel>();
            services.AddTransient<AcademicManagementViewModel>();
            services.AddTransient<ClassStudentListViewModel>();
            services.AddTransient<StudentManagementViewModel>();
            services.AddTransient<StudentDetailViewModel>();
            services.AddTransient<LoginViewModel>();

            // Views
            services.AddTransient<GradeManagementView>(provider => new GradeManagementView { DataContext = provider.GetRequiredService<GradeManagementViewModel>() });
            services.AddTransient<FinanceReportingView>(provider => new FinanceReportingView { DataContext = provider.GetRequiredService<FinanceReportingViewModel>() });
            services.AddTransient<CourseManagementView>(provider => new CourseManagementView { DataContext = provider.GetRequiredService<CourseManagementViewModel>() });
            services.AddTransient<CourseRegistrationView>(provider => new CourseRegistrationView { DataContext = provider.GetRequiredService<CourseRegistrationViewModel>() });
            services.AddTransient<AcademicManagementView>(provider => new AcademicManagementView { DataContext = provider.GetRequiredService<AcademicManagementViewModel>() });
            services.AddTransient<ClassStudentListView>();
            services.AddTransient<StudentManagementView>(provider => new StudentManagementView { DataContext = provider.GetRequiredService<StudentManagementViewModel>() });
            services.AddTransient<StudentDetailWindow>();
            services.AddTransient<LoginWindow>(provider => new LoginWindow
            {
                DataContext = provider.GetRequiredService<LoginViewModel>()
            });
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            
            // Subscribe to login success to show main window
            if (loginWindow.DataContext is LoginViewModel vm)
            {
                vm.OnLoginSuccess += () => 
                {
                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                    loginWindow.Close();
                };
            }

            loginWindow.Show();
        }
    }
}




