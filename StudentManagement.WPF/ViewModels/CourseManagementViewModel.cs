using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace StudentManagement.WPF.ViewModels
{
    public class CourseManagementViewModel : ViewModelBase
    {
        private readonly ICourseService _courseService;

        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        public ObservableCollection<Subject> Subjects { get => _subjects; set { _subjects = value; OnPropertyChanged(); } }
        public Subject? SelectedSubject { get; set; }

        private ObservableCollection<CourseSection> _sections = new ObservableCollection<CourseSection>();
        public ObservableCollection<CourseSection> Sections { get => _sections; set { _sections = value; OnPropertyChanged(); } }
        public CourseSection? SelectedSection { get; set; }

<<<<<<< HEAD
        public ICommand LoadDataCommand { get; }

        public CourseManagementViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());

            _ = LoadDataAsync();
=======
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
>>>>>>> origin/Main
        }

        private async Task LoadDataAsync()
        {
            try
            {
<<<<<<< HEAD
                var subjectsList = await _courseService.GetAllSubjectsAsync();
=======
                var subjectsList = await _courseService.GetAllSubjectsAsync(SearchText);
>>>>>>> origin/Main
                Subjects = new ObservableCollection<Subject>(subjectsList);

                var sectionsList = await _courseService.GetAllCourseSectionsAsync();
                Sections = new ObservableCollection<CourseSection>(sectionsList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading course data: {ex.Message}");
            }
        }
    }
}
