using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using StudentManagement.WPF.Commands;
using StudentManagement.WPF.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace StudentManagement.WPF.ViewModels
{
    public class AcademicManagementViewModel : ViewModelBase
    {
        private readonly IAcademicService _academicService;

        // Majors
        private ObservableCollection<Major> _majors = new ObservableCollection<Major>();
        public ObservableCollection<Major> Majors { get => _majors; set { _majors = value; OnPropertyChanged(); } }
        public Major? SelectedMajor { get; set; }

        private string _newMajorId = string.Empty;
        public string NewMajorId { get => _newMajorId; set { _newMajorId = value; OnPropertyChanged(); } }

        private string _newMajorName = string.Empty;
        public string NewMajorName { get => _newMajorName; set { _newMajorName = value; OnPropertyChanged(); } }

        // Classes
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        public ObservableCollection<Class> Classes { get => _classes; set { _classes = value; OnPropertyChanged(); } }
        public Class? SelectedClass { get; set; }

        private string _newClassId = string.Empty;
        public string NewClassId { get => _newClassId; set { _newClassId = value; OnPropertyChanged(); } }

        private string _newClassName = string.Empty;
        public string NewClassName { get => _newClassName; set { _newClassName = value; OnPropertyChanged(); } }

        private string _newClassAcademicYear = string.Empty;
        public string NewClassAcademicYear { get => _newClassAcademicYear; set { _newClassAcademicYear = value; OnPropertyChanged(); } }

        private Major? _newClassMajor;
        public Major? NewClassMajor { get => _newClassMajor; set { _newClassMajor = value; OnPropertyChanged(); } }

        // Lecturers
        private ObservableCollection<Lecturer> _lecturers = new ObservableCollection<Lecturer>();
        public ObservableCollection<Lecturer> Lecturers { get => _lecturers; set { _lecturers = value; OnPropertyChanged(); } }
        public Lecturer? SelectedLecturer { get; set; }

        private string _newLecturerId = string.Empty;
        public string NewLecturerId { get => _newLecturerId; set { _newLecturerId = value; OnPropertyChanged(); } }

        private string _newLecturerName = string.Empty;
        public string NewLecturerName { get => _newLecturerName; set { _newLecturerName = value; OnPropertyChanged(); } }

        private string _newLecturerEmail = string.Empty;
        public string NewLecturerEmail { get => _newLecturerEmail; set { _newLecturerEmail = value; OnPropertyChanged(); } }

        public Lecturer? HomeroomLecturerToAssign { get; set; }

        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }

        public ICommand LoadDataCommand { get; }
        public ICommand ViewClassStudentsCommand { get; }
        public ICommand AddMajorCommand { get; }
        public ICommand DeactivateMajorCommand { get; }
        public ICommand AddClassCommand { get; }
        public ICommand AssignHomeroomCommand { get; }
        public ICommand AddLecturerCommand { get; }
        public ICommand DeactivateLecturerCommand { get; }

        public AcademicManagementViewModel(IAcademicService academicService)
        {
            _academicService = academicService;
            LoadDataCommand = new RelayCommand(async _ => await LoadAllDataAsync());
            ViewClassStudentsCommand = new RelayCommand(_ => ViewClassStudents(), _ => SelectedClass != null);
            AddMajorCommand = new RelayCommand(async _ => await AddMajorAsync());
            DeactivateMajorCommand = new RelayCommand(async _ => await DeactivateMajorAsync(), _ => SelectedMajor != null);
            AddClassCommand = new RelayCommand(async _ => await AddClassAsync());
            AssignHomeroomCommand = new RelayCommand(async _ => await AssignHomeroomAsync(), _ => SelectedClass != null && HomeroomLecturerToAssign != null);
            AddLecturerCommand = new RelayCommand(async _ => await AddLecturerAsync());
            DeactivateLecturerCommand = new RelayCommand(async _ => await DeactivateLecturerAsync(), _ => SelectedLecturer != null);

            _ = LoadAllDataAsync();
        }

        private async Task LoadAllDataAsync()
        {
            try
            {
                var majorsList = await _academicService.GetAllMajorsAsync(SearchText);
                Majors = new ObservableCollection<Major>(majorsList);

                var classesList = await _academicService.GetAllClassesAsync();
                Classes = new ObservableCollection<Class>(classesList);

                var lecturersList = await _academicService.GetAllLecturersAsync(SearchText);
                Lecturers = new ObservableCollection<Lecturer>(lecturersList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading academic data: {ex.Message}");
            }
        }

        private void ViewClassStudents()
        {
            if (SelectedClass == null) return;
            var window = new ClassStudentListView { DataContext = new ClassStudentListViewModel(_academicService, SelectedClass.ClassId) };
            window.Show();
        }

        private async Task AddMajorAsync()
        {
            try
            {
                var (success, message) = await _academicService.AddMajorAsync(new Major { MajorId = NewMajorId, MajorName = NewMajorName, Status = MajorStatus.Active });
                MessageBox.Show(message);
                if (success) { NewMajorId = string.Empty; NewMajorName = string.Empty; await LoadAllDataAsync(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding major: {ex.Message}");
            }
        }

        private async Task DeactivateMajorAsync()
        {
            if (SelectedMajor == null) return;
            try
            {
                var (success, message) = await _academicService.DeactivateMajorAsync(SelectedMajor.MajorId);
                MessageBox.Show(message);
                if (success) await LoadAllDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating major: {ex.Message}");
            }
        }

        private async Task AddClassAsync()
        {
            if (NewClassMajor == null)
            {
                MessageBox.Show("Please select a major.");
                return;
            }

            try
            {
                var (success, message) = await _academicService.AddClassAsync(new Class
                {
                    ClassId = NewClassId,
                    ClassName = NewClassName,
                    MajorId = NewClassMajor.MajorId,
                    AcademicYear = NewClassAcademicYear,
                    Status = ClassStatus.Active
                });
                MessageBox.Show(message);
                if (success)
                {
                    NewClassId = string.Empty;
                    NewClassName = string.Empty;
                    NewClassAcademicYear = string.Empty;
                    NewClassMajor = null;
                    await LoadAllDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding class: {ex.Message}");
            }
        }

        private async Task AssignHomeroomAsync()
        {
            if (SelectedClass == null || HomeroomLecturerToAssign == null) return;
            try
            {
                var (success, message) = await _academicService.AssignHomeroomLecturerAsync(SelectedClass.ClassId, HomeroomLecturerToAssign.LecturerId);
                MessageBox.Show(message);
                if (success) await LoadAllDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning homeroom lecturer: {ex.Message}");
            }
        }

        private async Task AddLecturerAsync()
        {
            try
            {
                var (success, message) = await _academicService.AddLecturerAsync(new Lecturer
                {
                    LecturerId = NewLecturerId,
                    FullName = NewLecturerName,
                    Email = NewLecturerEmail,
                    Status = LecturerStatus.Active
                });
                MessageBox.Show(message);
                if (success)
                {
                    NewLecturerId = string.Empty;
                    NewLecturerName = string.Empty;
                    NewLecturerEmail = string.Empty;
                    await LoadAllDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding lecturer: {ex.Message}");
            }
        }

        private async Task DeactivateLecturerAsync()
        {
            if (SelectedLecturer == null) return;
            try
            {
                var (success, message) = await _academicService.DeactivateLecturerAsync(SelectedLecturer.LecturerId);
                MessageBox.Show(message);
                if (success) await LoadAllDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating lecturer: {ex.Message}");
            }
        }
    }
}
