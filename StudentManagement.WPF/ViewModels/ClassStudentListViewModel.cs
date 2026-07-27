using StudentManagement.Business.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.WPF.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudentManagement.WPF.ViewModels
{
    public class ClassStudentListViewModel : ViewModelBase
    {
        private readonly IAcademicService _academicService;
        private readonly string _classId;

        private ObservableCollection<Student> _students = new ObservableCollection<Student>();
        public ObservableCollection<Student> Students { get => _students; set { _students = value; OnPropertyChanged(); } }
        
        public int TotalStudents => Students.Count;

        public ClassStudentListViewModel(IAcademicService academicService, string classId)
        {
            _academicService = academicService;
            _classId = classId;

            _ = LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            var data = await _academicService.GetStudentsByClassAsync(_classId);
            Students = new ObservableCollection<Student>(data);
            OnPropertyChanged(nameof(TotalStudents));
        }
    }
}
