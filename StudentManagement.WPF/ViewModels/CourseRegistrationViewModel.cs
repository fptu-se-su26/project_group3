using StudentManagement.Business.Interfaces;
using StudentManagement.Business.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace StudentManagement.WPF.ViewModels
{
    public class CourseRegistrationViewModel : ViewModelBase
    {
        private readonly ICourseService _courseService;
        private readonly string _currentStudentId;
        private readonly string _currentSemesterId = "SP24"; // Hardcoded for mockup

        private ObservableCollection<CourseSection> _availableSections = new ObservableCollection<CourseSection>();
        public ObservableCollection<CourseSection> AvailableSections { get => _availableSections; set { _availableSections = value; OnPropertyChanged(); } }
        public CourseSection? SelectedAvailableSection { get; set; }

        private ObservableCollection<CourseSection> _timetable = new ObservableCollection<CourseSection>();
        public ObservableCollection<CourseSection> Timetable { get => _timetable; set { _timetable = value; OnPropertyChanged(); } }

        private int _totalCredits;
        public int TotalCredits { get => _totalCredits; set { _totalCredits = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand RefreshCommand { get; }

        public CourseRegistrationViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            _currentStudentId = SessionManager.Instance.CurrentUser?.Username ?? "SE150001"; // Fallback to a mock student

            RegisterCommand = new RelayCommand(async _ => await RegisterCourseAsync(), _ => SelectedAvailableSection != null);
            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var available = await _courseService.GetAvailableSectionsForSemesterAsync(_currentSemesterId);
                AvailableSections = new ObservableCollection<CourseSection>(available);

                var schedule = await _courseService.GetStudentTimetableAsync(_currentStudentId, _currentSemesterId);
                Timetable = new ObservableCollection<CourseSection>(schedule);

                TotalCredits = await _courseService.CalculateTotalCreditsAsync(_currentStudentId, _currentSemesterId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading registration data: {ex.Message}");
            }
        }

        private async Task RegisterCourseAsync()
        {
            if (SelectedAvailableSection == null) return;

            var (isSuccess, message) = await _courseService.RegisterCourseAsync(_currentStudentId, SelectedAvailableSection.SectionId);
            
            MessageBox.Show(message, isSuccess ? "Success" : "Registration Failed", MessageBoxButton.OK, isSuccess ? MessageBoxImage.Information : MessageBoxImage.Warning);
            
            if (isSuccess)
            {
                await LoadDataAsync();
            }
        }
    }
}
