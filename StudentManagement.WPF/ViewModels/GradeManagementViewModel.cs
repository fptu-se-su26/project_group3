using StudentManagement.Business.Interfaces;

using StudentManagement.Business.Services;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace StudentManagement.WPF.ViewModels
{
    public class GradeManagementViewModel : ViewModelBase
    {
        private readonly IFinanceGradeService _service;

        private readonly ICourseService _courseService;

        public ObservableCollection<CourseSection> Sections { get; } = new();

        private CourseSection? _selectedCourseSection;
        public CourseSection? SelectedCourseSection
        {
            get => _selectedCourseSection;
            set { _selectedCourseSection = value; OnPropertyChanged(); _ = LoadDataAsync(); }
        }

        private ObservableCollection<Grade> _grades = new ObservableCollection<Grade>();
        public ObservableCollection<Grade> Grades { get => _grades; set { _grades = value; OnPropertyChanged(); } }
        public Grade? SelectedGrade { get; set; }

        public ICommand LoadDataCommand { get; }
        public ICommand SaveGradeCommand { get; }


        public GradeManagementViewModel(IFinanceGradeService service, ICourseService courseService)
        {
            _service = service;
            _courseService = courseService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            SaveGradeCommand = new RelayCommand(async _ => await SaveGradeAsync(), _ => SelectedGrade != null);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var allSections = await _courseService.GetAllCourseSectionsAsync();
            var currentUser = SessionManager.Instance.CurrentUser;
            var isLecturer = currentUser?.Role.RoleName == "Lecturer";

            var sections = isLecturer
                ? allSections.Where(s => s.LecturerId == currentUser!.Username)
                : allSections;

            foreach (var s in sections) Sections.Add(s);
            SelectedCourseSection = Sections.Count > 0 ? Sections[0] : null;
        }

        private async Task LoadDataAsync()
        {

            if (SelectedCourseSection == null) return;
            try
            {
                var gradesList = await _service.GetGradesForSectionAsync(SelectedCourseSection.SectionId);
                Grades = new ObservableCollection<Grade>(gradesList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading grades: {ex.Message}");
            }
        }

        private async Task SaveGradeAsync()
        {
            if (SelectedGrade == null) return;
            try
            {

                var (success, message) = await _service.UpdateGradeAsync(SelectedGrade.GradeId, SelectedGrade.Assignment, SelectedGrade.ProgressTest, SelectedGrade.Practical, SelectedGrade.FinalExam);
                MessageBox.Show(message);
                if (success) await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving grade: {ex.Message}");
            }
        }
    }
}
