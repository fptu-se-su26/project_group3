using StudentManagement.Business.Interfaces;
using StudentManagement.WPF.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IAcademicService _academicService;
        private readonly ICourseService _courseService;

        private int _totalStudents;
        public int TotalStudents { get => _totalStudents; set { _totalStudents = value; OnPropertyChanged(); } }

        private int _totalLecturers;
        public int TotalLecturers { get => _totalLecturers; set { _totalLecturers = value; OnPropertyChanged(); } }

        private int _totalClasses;
        public int TotalClasses { get => _totalClasses; set { _totalClasses = value; OnPropertyChanged(); } }

        private int _totalMajors;
        public int TotalMajors { get => _totalMajors; set { _totalMajors = value; OnPropertyChanged(); } }

        private int _totalSubjects;
        public int TotalSubjects { get => _totalSubjects; set { _totalSubjects = value; OnPropertyChanged(); } }

        private int _activeStudents;
        public int ActiveStudents { get => _activeStudents; set { _activeStudents = value; OnPropertyChanged(); } }

        private int _graduatedStudents;
        public int GraduatedStudents { get => _graduatedStudents; set { _graduatedStudents = value; OnPropertyChanged(); } }

        public ICommand LoadDataCommand { get; }

        public DashboardViewModel(IAcademicService academicService, ICourseService courseService)
        {
            _academicService = academicService;
            _courseService = courseService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var summary = await _academicService.GetDashboardSummaryAsync();
                TotalStudents = summary.totalStudents;
                TotalLecturers = summary.totalLecturers;
                TotalClasses = summary.totalClasses;
                TotalMajors = summary.totalMajors;
                ActiveStudents = summary.activeStudents;
                GraduatedStudents = summary.graduatedStudents;

                var subjects = await _courseService.GetAllSubjectsAsync();
                TotalSubjects = System.Linq.Enumerable.Count(subjects);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}");
            }
        }
    }
}
