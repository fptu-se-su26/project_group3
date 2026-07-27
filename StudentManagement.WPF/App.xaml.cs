using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using StudentManagement.DataAccess;
using StudentManagement.DataAccess.Repositories;
using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Seeding;
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
            // Transient (not the EF Core default Scoped): a WPF app has no per-request scope, so a
            // Scoped context resolved from the root provider behaves like an app-lifetime singleton,
            // letting the ChangeTracker accumulate every entity ever loaded across every screen.
            services.AddDbContext<AppDbContext>(contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Transient);
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Services
            services.AddTransient<IAuditService, AuditService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFinanceGradeService, FinanceGradeService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IAcademicService, AcademicService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IAuthService, AuthService>();

            // ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<ChangePasswordViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<GradeManagementViewModel>();
            services.AddTransient<FinanceReportingViewModel>();
            services.AddTransient<CourseManagementViewModel>();
            services.AddTransient<CourseRegistrationViewModel>();
            services.AddTransient<AcademicManagementViewModel>();
            services.AddTransient<ClassStudentListViewModel>();
            services.AddTransient<StudentManagementViewModel>();
            services.AddTransient<LoginViewModel>();

            // Views
            services.AddTransient<DashboardView>(provider => new DashboardView { DataContext = provider.GetRequiredService<DashboardViewModel>() });
            services.AddTransient<UserManagementView>(provider => new UserManagementView { DataContext = provider.GetRequiredService<UserManagementViewModel>() });
            services.AddTransient<ChangePasswordWindow>(provider => new ChangePasswordWindow { DataContext = provider.GetRequiredService<ChangePasswordViewModel>() });
            services.AddTransient<ReportsView>(provider => new ReportsView { DataContext = provider.GetRequiredService<ReportsViewModel>() });
            services.AddTransient<GradeManagementView>(provider => new GradeManagementView { DataContext = provider.GetRequiredService<GradeManagementViewModel>() });
            services.AddTransient<FinanceReportingView>(provider => new FinanceReportingView { DataContext = provider.GetRequiredService<FinanceReportingViewModel>() });
            services.AddTransient<CourseManagementView>(provider => new CourseManagementView { DataContext = provider.GetRequiredService<CourseManagementViewModel>() });
            services.AddTransient<CourseRegistrationView>(provider => new CourseRegistrationView { DataContext = provider.GetRequiredService<CourseRegistrationViewModel>() });
            services.AddTransient<AcademicManagementView>(provider => new AcademicManagementView { DataContext = provider.GetRequiredService<AcademicManagementViewModel>() });
            services.AddTransient<ClassStudentListView>();
            services.AddTransient<StudentManagementView>(provider => new StudentManagementView { DataContext = provider.GetRequiredService<StudentManagementViewModel>() });
            services.AddTransient<LoginWindow>(provider => new LoginWindow
            {
                DataContext = provider.GetRequiredService<LoginViewModel>()
            });
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        await context.Database.MigrateAsync();
                        await DevDataSeeder.SeedAsync(context);
                    }).GetAwaiter().GetResult();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Could not connect to or initialize the database. Please verify SQL Server is running and reachable.\n\nDetails: {ex.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();

            if (loginWindow.DataContext is LoginViewModel vm)
            {
                vm.OnLoginSuccess += () =>
                {
                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.OnLogoutRequested += () =>
                    {
                        ShowLoginWindow();
                        mainWindow.Close();
                    };
                    mainWindow.Show();
                    loginWindow.Close();
                };
            }

            loginWindow.Show();
        }
    }
}
