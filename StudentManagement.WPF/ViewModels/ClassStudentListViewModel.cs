using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class ClassStudentListViewModel : ViewModelBase
    {
        private readonly IAcademicService _academicService;
        private readonly string _classId;
        private System.Collections.Generic.List<Student> _allStudents = new();

        private ObservableCollection<Student> _students = new ObservableCollection<Student>();
        public ObservableCollection<Student> Students { get => _students; set { _students = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalStudents)); } }

        public int TotalStudents => Students.Count;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
        }

        public ICommand SearchCommand { get; }

        public ClassStudentListViewModel(IAcademicService academicService, string classId)
        {
            _academicService = academicService;
            _classId = classId;
            SearchCommand = new RelayCommand(_ => ApplyFilter());

            _ = LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            var data = await _academicService.GetStudentsByClassAsync(_classId);
            _allStudents = data.ToList();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allStudents
                : _allStudents.Where(s => s.StudentId.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                                        || s.FullName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)).ToList();

            Students = new ObservableCollection<Student>(filtered);
        }
    }
}
