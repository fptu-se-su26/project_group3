using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;

namespace StudentManagement.WPF.ViewModels
{
    public class StudentManagementViewModel : ViewModelBase
    {
        private readonly IStudentService _studentService;
        
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

        public StudentManagementViewModel(IStudentService studentService)
        {
            _studentService = studentService;
            SearchCommand = new RelayCommand(async _ => await LoadDataAsync());
            AddCommand = new RelayCommand(_ => AddStudent());
            EditCommand = new RelayCommand(_ => EditStudent(), _ => SelectedStudent != null);
            ExportJsonCommand = new RelayCommand(async _ => await ExportJsonAsync());
            ExportXmlCommand = new RelayCommand(async _ => await ExportXmlAsync());
            ImportJsonCommand = new RelayCommand(async _ => await ImportJsonAsync());
            
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var data = await _studentService.GetAllStudentsAsync(SearchText);
            Students = new ObservableCollection<Student>(data);
        }

        private void AddStudent()
        {
            // Will open StudentDetailWindow
        }

        private void EditStudent()
        {
            // Will open StudentDetailWindow with SelectedStudent
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
                var (success, fail) = await _studentService.ImportFromJsonAsync(dlg.FileName);
                MessageBox.Show($"Import Complete.\nSuccess: {success}\nFailed/Duplicates: {fail}", "Import Result");
                await LoadDataAsync();
            }
        }
    }
}
