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

        public ObservableCollection<Semester> Semesters { get; } = new();

        private Semester? _selectedSemester;
        public Semester? SelectedSemester
        {
            get => _selectedSemester;
            set { _selectedSemester = value; OnPropertyChanged(); _ = LoadDataAsync(); }
        }

        private ObservableCollection<CourseSection> _availableSections = new ObservableCollection<CourseSection>();
        public ObservableCollection<CourseSection> AvailableSections { get => _availableSections; set { _availableSections = value; OnPropertyChanged(); } }
        public CourseSection? SelectedAvailableSection { get; set; }

        private ObservableCollection<CourseSection> _timetable = new ObservableCollection<CourseSection>();
        public ObservableCollection<CourseSection> Timetable { get => _timetable; set { _timetable = value; OnPropertyChanged(); } }
        public CourseSection? SelectedTimetableEntry { get; set; }

        private int _totalCredits;
        public int TotalCredits { get => _totalCredits; set { _totalCredits = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }

        public CourseRegistrationViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            _currentStudentId = SessionManager.Instance.CurrentUser?.Username ?? "SE150001";

            RegisterCommand = new RelayCommand(async _ => await RegisterCourseAsync(), _ => SelectedAvailableSection != null);
            CancelCommand = new RelayCommand(async _ => await CancelCourseAsync(), _ => SelectedTimetableEntry != null);
            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var semesters = await _courseService.GetAllSemestersAsync();
            foreach (var s in semesters) Semesters.Add(s);
            SelectedSemester = Semesters.Count > 0 ? Semesters[0] : null;
        }

        private async Task LoadDataAsync()
        {
            if (SelectedSemester == null) return;
            try
            {
                var available = await _courseService.GetAvailableSectionsForSemesterAsync(SelectedSemester.SemesterId);
                AvailableSections = new ObservableCollection<CourseSection>(available);

                var schedule = await _courseService.GetStudentTimetableAsync(_currentStudentId, SelectedSemester.SemesterId);
                Timetable = new ObservableCollection<CourseSection>(schedule);

                TotalCredits = await _courseService.CalculateTotalCreditsAsync(_currentStudentId, SelectedSemester.SemesterId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading registration data: {ex.Message}");
            }
        }

        private async Task RegisterCourseAsync()
        {
            if (SelectedAvailableSection == null) return;

            try
            {
                var (isSuccess, message) = await _courseService.RegisterCourseAsync(_currentStudentId, SelectedAvailableSection.SectionId);

                MessageBox.Show(message, isSuccess ? "Success" : "Registration Failed", MessageBoxButton.OK, isSuccess ? MessageBoxImage.Information : MessageBoxImage.Warning);

                if (isSuccess)
                {
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error registering course: {ex.Message}");
            }
        }

        private async Task CancelCourseAsync()
        {
            if (SelectedTimetableEntry == null) return;

            try
            {
                var (isSuccess, message) = await _courseService.CancelRegistrationAsync(_currentStudentId, SelectedTimetableEntry.SectionId);
                MessageBox.Show(message);
                if (isSuccess) await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling registration: {ex.Message}");
            }
        }
    }
}
