using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Enums;
using StudentManagement.WPF.Commands;
using StudentManagement.WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;

namespace StudentManagement.WPF.ViewModels
{
    public class StudentManagementViewModel : ViewModelBase
    {
        private readonly IStudentService _studentService;
        private readonly IAcademicService _academicService;
        private readonly IServiceProvider _serviceProvider;

        private ObservableCollection<Student> _students = new ObservableCollection<Student>();
        public ObservableCollection<Student> Students
        {
            get => _students;
            set { _students = value; OnPropertyChanged(); }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Class> Classes { get; } = new();
        public ObservableCollection<Major> Majors { get; } = new();
        public ObservableCollection<StudentStatus?> StatusOptions { get; } = new(new StudentStatus?[] { null }.Concat(Enum.GetValues<StudentStatus>().Cast<StudentStatus?>()));

        public Class? SelectedClassFilter { get; set; }
        public Major? SelectedMajorFilter { get; set; }
        public StudentStatus? SelectedStatusFilter { get; set; }

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set { _selectedStudent = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ExportXmlCommand { get; }
        public ICommand ImportJsonCommand { get; }

        public StudentManagementViewModel(IStudentService studentService, IAcademicService academicService, IServiceProvider serviceProvider)
        {
            _studentService = studentService;
            _academicService = academicService;
            _serviceProvider = serviceProvider;

            SearchCommand = new RelayCommand(async _ => await LoadDataAsync());
            AddCommand = new RelayCommand(async _ => await OpenDetailWindowAsync(null));
            EditCommand = new RelayCommand(async _ => await OpenDetailWindowAsync(SelectedStudent), _ => SelectedStudent != null);
            ExportJsonCommand = new RelayCommand(async _ => await ExportJsonAsync());
            ExportXmlCommand = new RelayCommand(async _ => await ExportXmlAsync());
            ImportJsonCommand = new RelayCommand(async _ => await ImportJsonAsync());

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var classes = await _academicService.GetAllClassesAsync();
            foreach (var c in classes) Classes.Add(c);

            var majors = await _academicService.GetAllMajorsAsync();
            foreach (var m in majors) Majors.Add(m);

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var data = await _studentService.GetAllStudentsAsync(SearchText, SelectedClassFilter?.ClassId, SelectedMajorFilter?.MajorId, SelectedStatusFilter);
            Students = new ObservableCollection<Student>(data);
        }

        private async Task OpenDetailWindowAsync(Student? student)
        {
            var vm = new StudentDetailViewModel(_serviceProvider.GetRequiredService<IStudentService>(), student);
            var window = new StudentDetailWindow { DataContext = vm };
            vm.OnRequestClose += () => window.Close();
            window.ShowDialog();
            await LoadDataAsync();
        }

        private async Task ExportJsonAsync()
        {
            var dlg = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", DefaultExt = ".json" };
            if (dlg.ShowDialog() == true)
            {
                await _studentService.ExportToJsonAsync(dlg.FileName, Students);
                MessageBox.Show("Exported successfully to JSON", "Success");
            }
        }

        private async Task ExportXmlAsync()
        {
            var dlg = new SaveFileDialog { Filter = "XML files (*.xml)|*.xml", DefaultExt = ".xml" };
            if (dlg.ShowDialog() == true)
            {
                await _studentService.ExportToXmlAsync(dlg.FileName, Students);
                MessageBox.Show("Exported successfully to XML", "Success");
            }
        }

        private async Task ImportJsonAsync()
        {
            var dlg = new OpenFileDialog { Filter = "JSON files (*.json)|*.json" };
            if (dlg.ShowDialog() == true)
            {
                var (success, fail, errors) = await _studentService.ImportFromJsonAsync(dlg.FileName);
                var errorText = errors.Any() ? "\n\nDetails:\n" + string.Join("\n", errors) : "";
                MessageBox.Show($"Import Complete.\nSuccess: {success}\nFailed/Duplicates: {fail}{errorText}", "Import Result");
                await LoadDataAsync();
            }
        }
    }
}
