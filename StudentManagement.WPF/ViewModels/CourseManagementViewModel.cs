using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;
using System.Globalization;

namespace StudentManagement.WPF.ViewModels
{
    public class CourseManagementViewModel : ViewModelBase
    {
        private readonly ICourseService _courseService;
        private readonly IAcademicService _academicService;

        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        public ObservableCollection<Subject> Subjects { get => _subjects; set { _subjects = value; OnPropertyChanged(); } }
        public Subject? SelectedSubject { get; set; }

        private ObservableCollection<CourseSection> _sections = new ObservableCollection<CourseSection>();
        public ObservableCollection<CourseSection> Sections { get => _sections; set { _sections = value; OnPropertyChanged(); } }
        public CourseSection? SelectedSection { get; set; }

        public ICommand LoadDataCommand { get; }

        public CourseManagementViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());

            _ = LoadDataAsync();
        public ObservableCollection<Lecturer> Lecturers { get; } = new();
        public ObservableCollection<Semester> Semesters { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

        private string _newSubjectId = string.Empty;
        public string NewSubjectId { get => _newSubjectId; set { _newSubjectId = value; OnPropertyChanged(); } }

        private string _newSubjectName = string.Empty;
        public string NewSubjectName { get => _newSubjectName; set { _newSubjectName = value; OnPropertyChanged(); } }

        private int _newSubjectCredits = 3;
        public int NewSubjectCredits { get => _newSubjectCredits; set { _newSubjectCredits = value; OnPropertyChanged(); } }

        private string _newSectionId = string.Empty;
        public string NewSectionId { get => _newSectionId; set { _newSectionId = value; OnPropertyChanged(); } }

        private Subject? _newSectionSubject;
        public Subject? NewSectionSubject { get => _newSectionSubject; set { _newSectionSubject = value; OnPropertyChanged(); } }

        private Semester? _newSectionSemester;
        public Semester? NewSectionSemester { get => _newSectionSemester; set { _newSectionSemester = value; OnPropertyChanged(); } }

        private string _newSectionRoom = string.Empty;
        public string NewSectionRoom { get => _newSectionRoom; set { _newSectionRoom = value; OnPropertyChanged(); } }

        private string _newSectionDay = "Monday";
        public string NewSectionDay { get => _newSectionDay; set { _newSectionDay = value; OnPropertyChanged(); } }

        private string _newSectionStartTime = "07:00";
        public string NewSectionStartTime { get => _newSectionStartTime; set { _newSectionStartTime = value; OnPropertyChanged(); } }

        private string _newSectionEndTime = "09:00";
        public string NewSectionEndTime { get => _newSectionEndTime; set { _newSectionEndTime = value; OnPropertyChanged(); } }

        private int _newSectionCapacity = 40;
        public int NewSectionCapacity { get => _newSectionCapacity; set { _newSectionCapacity = value; OnPropertyChanged(); } }

        public Lecturer? LecturerToAssign { get; set; }

        public ICommand LoadDataCommand { get; }
        public ICommand AddSubjectCommand { get; }
        public ICommand DeactivateSubjectCommand { get; }
        public ICommand AddSectionCommand { get; }
        public ICommand AssignLecturerCommand { get; }

        public CourseManagementViewModel(ICourseService courseService, IAcademicService academicService)
        {
            _courseService = courseService;
            _academicService = academicService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            AddSubjectCommand = new RelayCommand(async _ => await AddSubjectAsync());
            DeactivateSubjectCommand = new RelayCommand(async _ => await DeactivateSubjectAsync(), _ => SelectedSubject != null);
            AddSectionCommand = new RelayCommand(async _ => await AddSectionAsync());
            AssignLecturerCommand = new RelayCommand(async _ => await AssignLecturerAsync(), _ => SelectedSection != null && LecturerToAssign != null);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                var lecturers = await _academicService.GetAllLecturersAsync();
                foreach (var l in lecturers) Lecturers.Add(l);

                var semesters = await _courseService.GetAllSemestersAsync();
                foreach (var s in semesters) Semesters.Add(s);

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing course management: {ex.Message}");
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var subjectsList = await _courseService.GetAllSubjectsAsync();
                var subjectsList = await _courseService.GetAllSubjectsAsync(SearchText);
                Subjects = new ObservableCollection<Subject>(subjectsList);

                var sectionsList = await _courseService.GetAllCourseSectionsAsync();
                Sections = new ObservableCollection<CourseSection>(sectionsList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading course data: {ex.Message}");
            }
        }

        private async Task AddSubjectAsync()
        {
            try
            {
                var (success, message) = await _courseService.AddSubjectAsync(new Subject
                {
                    SubjectId = NewSubjectId,
                    SubjectName = NewSubjectName,
                    Credits = NewSubjectCredits,
                    Status = SubjectStatus.Active
                });
                MessageBox.Show(message);
                if (success)
                {
                    NewSubjectId = string.Empty;
                    NewSubjectName = string.Empty;
                    NewSubjectCredits = 3;
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding subject: {ex.Message}");
            }
        }

        private async Task DeactivateSubjectAsync()
        {
            if (SelectedSubject == null) return;
            try
            {
                var (success, message) = await _courseService.DeactivateSubjectAsync(SelectedSubject.SubjectId);
                MessageBox.Show(message);
                if (success) await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating subject: {ex.Message}");
            }
        }

        private async Task AddSectionAsync()
        {
            if (NewSectionSubject == null || NewSectionSemester == null)
            {
                MessageBox.Show("Please select a subject and semester.");
                return;
            }

            if (!TimeSpan.TryParseExact(NewSectionStartTime, "hh\\:mm", CultureInfo.InvariantCulture, out var startTime)
                || !TimeSpan.TryParseExact(NewSectionEndTime, "hh\\:mm", CultureInfo.InvariantCulture, out var endTime))
            {
                MessageBox.Show("Start/End time must be in HH:mm format (e.g. 07:00).");
                return;
            }

            if (endTime <= startTime)
            {
                MessageBox.Show("End time must be after start time.");
                return;
            }

            try
            {
                var (success, message) = await _courseService.AddCourseSectionAsync(new CourseSection
                {
                    SectionId = NewSectionId,
                    SubjectId = NewSectionSubject.SubjectId,
                    SemesterId = NewSectionSemester.SemesterId,
                    Room = NewSectionRoom,
                    DayOfWeek = NewSectionDay,
                    StartTime = startTime,
                    EndTime = endTime,
                    Capacity = NewSectionCapacity,
                    Status = CourseSectionStatus.Opened
                });
                MessageBox.Show(message);
                if (success)
                {
                    NewSectionId = string.Empty;
                    NewSectionSubject = null;
                    NewSectionSemester = null;
                    NewSectionRoom = string.Empty;
                    NewSectionDay = "Monday";
                    NewSectionStartTime = "07:00";
                    NewSectionEndTime = "09:00";
                    NewSectionCapacity = 40;
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding course section: {ex.Message}");
            }
        }

        private async Task AssignLecturerAsync()
        {
            if (SelectedSection == null || LecturerToAssign == null) return;
            try
            {
                var (success, message) = await _courseService.AssignLecturerToSectionAsync(SelectedSection.SectionId, LecturerToAssign.LecturerId);
                MessageBox.Show(message);
                if (success) await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning lecturer: {ex.Message}");
            }
        }
    }
}
