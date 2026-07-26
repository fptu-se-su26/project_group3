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

        public ICommand LoadDataCommand { get; }

        public CourseManagementViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var subjectsList = await _courseService.GetAllSubjectsAsync();
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
