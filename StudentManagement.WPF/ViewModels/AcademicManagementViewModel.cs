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
    public class AcademicManagementViewModel : ViewModelBase
    {
        private readonly IAcademicService _academicService;

        // Majors
        private ObservableCollection<Major> _majors = new ObservableCollection<Major>();
        public ObservableCollection<Major> Majors { get => _majors; set { _majors = value; OnPropertyChanged(); } }
        public Major? SelectedMajor { get; set; }

        // Classes
        private ObservableCollection<Class> _classes = new ObservableCollection<Class>();
        public ObservableCollection<Class> Classes { get => _classes; set { _classes = value; OnPropertyChanged(); } }
        public Class? SelectedClass { get; set; }

        // Lecturers
        private ObservableCollection<Lecturer> _lecturers = new ObservableCollection<Lecturer>();
        public ObservableCollection<Lecturer> Lecturers { get => _lecturers; set { _lecturers = value; OnPropertyChanged(); } }
        public Lecturer? SelectedLecturer { get; set; }

        public ICommand LoadDataCommand { get; }
        public ICommand ViewClassStudentsCommand { get; }

        public AcademicManagementViewModel(IAcademicService academicService)
        {
            _academicService = academicService;
            LoadDataCommand = new RelayCommand(async _ => await LoadAllDataAsync());
            ViewClassStudentsCommand = new RelayCommand(_ => ViewClassStudents(), _ => SelectedClass != null);

            _ = LoadAllDataAsync();
        }

        private async Task LoadAllDataAsync()
        {
            try
            {
                var majorsList = await _academicService.GetAllMajorsAsync();
                Majors = new ObservableCollection<Major>(majorsList);

                var classesList = await _academicService.GetAllClassesAsync();
                Classes = new ObservableCollection<Class>(classesList);

                var lecturersList = await _academicService.GetAllLecturersAsync();
                Lecturers = new ObservableCollection<Lecturer>(lecturersList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading academic data: {ex.Message}");
            }
        }

        private void ViewClassStudents()
        {
            // Code to open ClassStudentListView with SelectedClass
        }
    }
}
